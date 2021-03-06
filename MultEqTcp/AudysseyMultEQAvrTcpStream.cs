using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Buffers.Binary;

namespace Audyssey
{
    namespace MultEQTcpStream
    {
        public delegate void AudysseyMultEQAvrTcpStreamParseCallback(char TransmitReceiveChar, string CmdString, byte[] DataByte, byte CurrentSegment, byte TotalSegments, bool SegmentComplete);

        public class AudysseyMultEQAvrTcpStream
        {
            private char _TransmitReceive;
            private UInt16 _TotalLength;
            private byte _CurrentSegment;
            private byte _TotalSegments;
            private UInt16 _DataLength;

            private byte[][] DataByteJaggedArray = null;
            private int DataByteJaggedArrayTotalLength;
            private int CountedPackets;

            private const byte ESC = 27;
            private const byte EOT = 4;
            private const byte NUL = 0;

            private AudysseyMultEQAvrTcpStreamParseCallback _AudysseyMultEQAvrTcpStreamParseCallback = null;

            public AudysseyMultEQAvrTcpStream(AudysseyMultEQAvrTcpStreamParseCallback AudysseyMultEQAvrTcpStreamParseCallback = null)
            {
                _AudysseyMultEQAvrTcpStreamParseCallback = AudysseyMultEQAvrTcpStreamParseCallback;
            }

            public void Unpack(byte[] packetData, ushort packetLength)
            {
                if (packetLength > 18)
                {
                    try
                    {
                        MemoryStream memoryStream = new(packetData, 0, packetLength);

                        // If we want to filter only packets which we can decode the minimum
                        // length of a packet with no data is header + checksum = 19 bytes.
                        if (memoryStream.Length > 18)
                        {
                            byte[] array = memoryStream.ToArray();
                            Array.Resize<byte>(ref array, array.Length - 1);
                            byte CheckSum = CalculateChecksum(array);

                            if (memoryStream.Length > 18)
                            {
                                BinaryReader binaryReader = new(memoryStream);
                                _TransmitReceive = binaryReader.ReadChar();
                                _TotalLength = BinaryPrimitives.ReverseEndianness(binaryReader.ReadUInt16());
                                _CurrentSegment = binaryReader.ReadByte();
                                _TotalSegments = binaryReader.ReadByte();
                                string Command = Encoding.ASCII.GetString(binaryReader.ReadBytes(10));
                                byte Reserved = binaryReader.ReadByte();
                                _DataLength = BinaryPrimitives.ReverseEndianness(binaryReader.ReadUInt16());
                                byte[] Data = binaryReader.ReadBytes(_DataLength);
                                if (CheckSum == binaryReader.ReadByte())
                                {
                                    bool SegmentComplete = true;
                                    // binary transfers have multiple segmemts
                                    if (_TotalSegments != 0)
                                    {
                                        // for multi segment transfers we need to report if we received all (possible out-of-order) segments
                                        SegmentComplete = false;
                                        // upon receiving the first segment (who can be out of order!)
                                        if (_CurrentSegment == 0)
                                        {
                                            // we know how many segments there are
                                            DataByteJaggedArray = new byte[_TotalSegments + 1][];
                                            // accumulate length for later unpacking
                                            DataByteJaggedArrayTotalLength = 0;
                                            // count the segments, they can be out of order, so we even might receive the last one premature
                                            CountedPackets = 0;
                                        }
                                        // upon receiving any other than first segment
                                        else
                                        {
                                            // if we have no array yet we missed the first segment (maybe it is out-of-order)
                                            if (DataByteJaggedArray == null)
                                            {
                                                // but we know how many segments there are
                                                DataByteJaggedArray = new byte[_TotalSegments + 1][];
                                                // accumulate length for later unpacking
                                                DataByteJaggedArrayTotalLength = 0;
                                                // count the segments, they can be out of order, so we even might receive the last one premature
                                                CountedPackets = 0;
                                            }
                                            else
                                            {
                                                CountedPackets++;
                                            }
                                        }
                                        // we know the length of this segment (however they likely are all of equal length)
                                        DataByteJaggedArray[_CurrentSegment] = new byte[Data.Length];
                                        // copy the current segment at its sequence number
                                        DataByteJaggedArray[_CurrentSegment] = Data;
                                        // and of course update the total length
                                        DataByteJaggedArrayTotalLength += Data.Length;
                                        // upon receiving the final segment
                                        if (CountedPackets == _TotalSegments) //((CurrentPacket == TotalPackets) && (DataByteJaggedArrayTotalLength == TotalPackets * DataByte.Length))
                                        {
                                            // we know the total length of all segments
                                            Data = new byte[DataByteJaggedArrayTotalLength];
                                            // track the position in the new array data is copied
                                            var PreviousBytaDataLength = 0;
                                            // iterate all segments in their original order
                                            foreach (byte[] DataByteArray in DataByteJaggedArray)
                                            {
                                                // wtf do we sometimes miss a segment????
                                                if (DataByteArray != null)
                                                {
                                                    //Append the new array to the current array
                                                    Array.Copy(DataByteArray, 0, Data, PreviousBytaDataLength, DataByteArray.Length);
                                                    // track the position in the new array data is copied
                                                    PreviousBytaDataLength += DataByteArray.Length;
                                                }
                                            }
                                            // reset for next multi segment transfer
                                            DataByteJaggedArray = null;
                                            // accumulate length for later unpacking
                                            DataByteJaggedArrayTotalLength = 0;
                                            // count the segments, they can be out of order, so we even might receive the last one premature
                                            CountedPackets = 0;
                                            // pass assemled packet with data
                                            SegmentComplete = true;
                                        }
                                        else
                                        {
                                            // do not pass incomplete dataset but report Command, CurrentPacket and TotalPackets
                                            Data = Encoding.ASCII.GetBytes(Data.Length.ToString());
                                        }
                                    }
                                    // callback to populate function
                                    _AudysseyMultEQAvrTcpStreamParseCallback?.Invoke(_TransmitReceive, Command, Data, _CurrentSegment, _TotalSegments, SegmentComplete);
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else if (packetLength == 1)
                {
                    Console.Write(packetData[0].ToString());
                    switch (packetData[0])
                    {
                        case ESC:
                            break;
                        case EOT:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ;
                }
            }

            private byte CalculateChecksum(byte[] dataToCalculate)
            {
                return dataToCalculate.Aggregate((r, n) => r += n);
            }
        }
    }
}