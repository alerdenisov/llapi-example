using UnityEngine;
using System.Collections;
using System.IO;

public class UnnyNetPacker {

	enum TypeTags
	{
		IntTag,
        FloatTag,
        Vector3Tag,
		UIntTag,
		LongTag,
		ULongTag,
		ShortTag,
		UShortTag,
		ByteTag,
        SByteTag,
        BoolTag,
		StringTag,
        UShortArraySmall,
        UShortArrayBig,
	}

	public const short HeaderSize = 2 + 1;//netId + cmd id

    static public object[] UnpackObject(byte[] bytes, int bytesCount)
    {
        object[] arg = new object[1];
        arg[0] = 0;
        int size = 0;
        int num = HeaderSize;
        try
        {
            while (num < bytesCount)
            {
                switch ((TypeTags)bytes[num++])
                {
                    case TypeTags.IntTag:
                        {
                            if (num + 4 <= bytesCount)
                            {
                                int intValue = System.BitConverter.ToInt32(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 4;
                            break;
                        }
                    case TypeTags.FloatTag:
                        {
                            if (num + 4 <= bytesCount)
                            {
                                var floatValue = System.BitConverter.ToSingle(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = floatValue;
                            }
                            num += 4;
                            break;
                        }
                    case TypeTags.Vector3Tag:
                        {
                            if (num + 12 <= bytesCount)
                            {
                                var vecValue = new Vector3();
                                for (int axis = 0; axis < 3; axis++)
                                {
                                    vecValue[axis] = System.BitConverter.ToSingle(bytes, num + (4 * axis));
                                }

                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = vecValue;
                            }
                            num += 12;
                            break;
                        }
                    case TypeTags.UIntTag:
                        {
                            if (num + 4 <= bytesCount)
                            {
                                uint intValue = System.BitConverter.ToUInt32(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 4;
                            break;
                        }
                    case TypeTags.LongTag:
                        {
                            if (num + 8 <= bytesCount)
                            {
                                long intValue = System.BitConverter.ToInt64(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 8;
                            break;
                        }
                    case TypeTags.ULongTag:
                        {
                            if (num + 8 <= bytesCount)
                            {
                                ulong intValue = System.BitConverter.ToUInt64(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 8;
                            break;
                        }
                    case TypeTags.ShortTag:
                        {
                            if (num + 2 <= bytesCount)
                            {
                                short intValue = System.BitConverter.ToInt16(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 2;
                            break;
                        }
                    case TypeTags.UShortTag:
                        {
                            if (num + 2 <= bytesCount)
                            {
                                ushort intValue = System.BitConverter.ToUInt16(bytes, num);
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = intValue;
                            }
                            num += 2;
                            break;
                        }
                    case TypeTags.ByteTag:
                        {
                            System.Array.Resize(ref arg, size + 1);
                            arg[size++] = bytes[num++];
                            break;
                        }
                    case TypeTags.SByteTag:
                        {
                            System.Array.Resize(ref arg, size + 1);
                            arg[size++] = (sbyte)bytes[num++];
                            break;
                        }
                    case TypeTags.BoolTag:
                        {
                            System.Array.Resize(ref arg, size + 1);
                            arg[size++] = (bytes[num++] == 1);
                            break;
                        }
                    case TypeTags.StringTag:
                        {
                            if (num + 2 <= bytesCount)
                            {
                                ushort intValue = System.BitConverter.ToUInt16(bytes, num);
                                num += 2;
                                if (num + intValue <= bytesCount)
                                {
                                    System.Array.Resize(ref arg, size + 1);
                                    arg[size++] = System.Text.Encoding.UTF8.GetString(bytes, num, intValue);
                                }
                                num += intValue;
                            }
                            else
                                num += 2;
                            break;
                        }
                    case TypeTags.UShortArraySmall:
                        {
                            byte length = bytes[num++];
                            if (num + 2 * length <= bytesCount)
                            {
                                ushort[] arr = new ushort[length];
                                for (int j = 0; j < length; j++)
                                {
                                    arr[j] = System.BitConverter.ToUInt16(bytes, num);
                                    num += 2;
                                }
                                System.Array.Resize(ref arg, size + 1);
                                arg[size++] = arr;
                            }
                            else
                                num += 2 * length;
                            break;
                        }
                    case TypeTags.UShortArrayBig:
                        {
                            if (num + 2 <= bytesCount)
                            {
                                ushort length = System.BitConverter.ToUInt16(bytes, num);
                                num += 2;

                                if (num + 2 * length <= bytesCount)
                                {
                                    ushort[] arr = new ushort[length];

                                    for (int j = 0; j < length; j++)
                                    {
                                        arr[j] = System.BitConverter.ToUInt16(bytes, num);
                                        num += 2;
                                    }
                                    System.Array.Resize(ref arg, size + 1);
                                    arg[size++] = arr;
                                }
                                else
                                    num += 2 * length;
                            }
                            else
                                num += 2;
                            break;
                        }
                    default:
                        Debug.Log("Couldn't unpack");
                        return arg;
                }
            }
        }
        catch (System.ArgumentException e)
        {
            Debug.Log("e = " + e);
        }
        return arg;
    }

    const int BufferSize = 1024 * 32;
    static byte [] m_Buffer = new byte[BufferSize];

    static public byte[] PackObject(out int size, params object []arg)
	{
        byte [] bytes = m_Buffer;//new byte[HeaderSize];
		size = HeaderSize;
        for (int i = 0; i < arg.Length; i++)
        {
            if (arg[i] is int)
            {
                int intValue = (int)arg[i];
                //				System.Array.Resize(ref bytes, size + 5);
                bytes[size++] = (byte)TypeTags.IntTag;
                bytes[size++] = (byte)intValue;
                bytes[size++] = (byte)(intValue >> 8);
                bytes[size++] = (byte)(intValue >> 16);
                bytes[size++] = (byte)(intValue >> 24);
            }
            else if (arg[i] is float)
            {
                var floatValue = (float)arg[i];
                var floatBytes = System.BitConverter.GetBytes(floatValue);
                //				System.Array.Resize(ref bytes, size + 5);
                bytes[size++] = (byte)TypeTags.FloatTag;
                bytes[size++] = floatBytes[0];
                bytes[size++] = floatBytes[1];
                bytes[size++] = floatBytes[2];
                bytes[size++] = floatBytes[3];
            }
            else if(arg[i] is Vector3)
            {
                var vecValue = (Vector3)arg[i];
                bytes[size++] = (byte)TypeTags.Vector3Tag;
                for (int axis = 0; axis < 3; axis++)
                {
                    var floatBytes = System.BitConverter.GetBytes(vecValue[axis]);
                    bytes[size++] = floatBytes[0];
                    bytes[size++] = floatBytes[1];
                    bytes[size++] = floatBytes[2];
                    bytes[size++] = floatBytes[3];
                }
            }
            else if (arg[i] is uint)
            {
                uint intValue = (uint)arg[i];
                //					System.Array.Resize(ref bytes, size + 5);
                bytes[size++] = (byte)TypeTags.UIntTag;
                bytes[size++] = (byte)intValue;
                bytes[size++] = (byte)(intValue >> 8);
                bytes[size++] = (byte)(intValue >> 16);
                bytes[size++] = (byte)(intValue >> 24);
            }
            else if (arg[i] is short)
            {
                short shortValue = (short)arg[i];
                //						System.Array.Resize(ref bytes, size + 3);
                bytes[size++] = (byte)TypeTags.ShortTag;
                bytes[size++] = (byte)shortValue;
                bytes[size++] = (byte)(shortValue >> 8);
            }
            else if (arg[i] is ushort)
            {
                ushort shortValue = (ushort)arg[i];
                //							System.Array.Resize(ref bytes, size + 3);
                bytes[size++] = (byte)TypeTags.UShortTag;
                bytes[size++] = (byte)shortValue;
                bytes[size++] = (byte)(shortValue >> 8);
            }
            else if (arg[i] is byte)
            {
                //								System.Array.Resize(ref bytes, size + 2);
                bytes[size++] = (byte)TypeTags.ByteTag;
                bytes[size++] = (byte)arg[i];
            }
            else if (arg[i] is sbyte)
            {
                //                                    System.Array.Resize(ref bytes, size + 2);
                bytes[size++] = (byte)TypeTags.SByteTag;
                bytes[size++] = (byte)(sbyte)arg[i];
            }
            else if (arg[i] is long)
            {
                long intValue = (long)arg[i];
                //										System.Array.Resize(ref bytes, size + 1+8);
                bytes[size++] = (byte)TypeTags.LongTag;
                bytes[size++] = (byte)intValue;
                bytes[size++] = (byte)(intValue >> 8);
                bytes[size++] = (byte)(intValue >> 16);
                bytes[size++] = (byte)(intValue >> 24);
                bytes[size++] = (byte)(intValue >> 32);
                bytes[size++] = (byte)(intValue >> 40);
                bytes[size++] = (byte)(intValue >> 48);
                bytes[size++] = (byte)(intValue >> 56);
            }
            else if (arg[i] is ulong)
            {
                ulong intValue = (ulong)arg[i];
                //											System.Array.Resize(ref bytes, size + 1+8);
                bytes[size++] = (byte)TypeTags.ULongTag;
                bytes[size++] = (byte)intValue;
                bytes[size++] = (byte)(intValue >> 8);
                bytes[size++] = (byte)(intValue >> 16);
                bytes[size++] = (byte)(intValue >> 24);
                bytes[size++] = (byte)(intValue >> 32);
                bytes[size++] = (byte)(intValue >> 40);
                bytes[size++] = (byte)(intValue >> 48);
                bytes[size++] = (byte)(intValue >> 56);
            }
            else if (arg[i] is bool)
            {
                //		                                        System.Array.Resize(ref bytes, size + 2);
                bytes[size++] = (byte)TypeTags.BoolTag;
                bytes[size++] = ((bool)arg[i]) ? (byte)1 : (byte)0;
            }
            else if (arg[i] is ushort[])
            {
                ushort[] shortValue = (ushort[])arg[i];
                if (shortValue.Length <= byte.MaxValue)
                {
                    //		                                                System.Array.Resize(ref bytes, size + 1 + 1 + 2*shortValue.Length);
                    bytes[size++] = (byte)TypeTags.UShortArraySmall;
                    bytes[size++] = (byte)shortValue.Length;
                }
                else
                {
                    //		                                                System.Array.Resize(ref bytes, size + 1 + 2 + 2*shortValue.Length);
                    bytes[size++] = (byte)TypeTags.UShortArrayBig;
                    bytes[size++] = (byte)shortValue.Length;
                    bytes[size++] = (byte)(shortValue.Length >> 8);
                }
                for (int j = 0; j < shortValue.Length; j++)
                {
                    bytes[size++] = (byte)shortValue[j];
                    bytes[size++] = (byte)(shortValue[j] >> 8);
                }
            }
            else if (arg[i] is string)
            {
                string str = (string)arg[i];
                byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                int newSize = strBytes.Length;
                //		                									System.Array.Resize(ref bytes, size + newSize + 3);
                bytes[size++] = (byte)TypeTags.StringTag;
                ushort shortValue = (ushort)newSize;
                bytes[size++] = (byte)shortValue;
                bytes[size++] = (byte)(shortValue >> 8);
                System.Buffer.BlockCopy(strBytes, 0, bytes, size, newSize);
                size += newSize;
            }
        }
		return bytes;
	}
}
