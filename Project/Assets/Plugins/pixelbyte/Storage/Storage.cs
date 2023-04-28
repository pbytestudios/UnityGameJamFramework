//#define USE_SEVENZIP
//#define USE_SHARPZIPLIB

//Can't use compression in winStore apps...:<
#if UNITY_WINRT
#undef USE_SHARPZIPLIB
#undef USE_SEVENZIP
using WinStore;
#endif

//In order to get rid of unibiler for stanalone builds, we will use
//7zip for the standalone builds
//#if UNITY_STANDALONE
//#undef USE_SHARPZIPLIB
//#define USE_SEVENZIP
//#endif

using UnityEngine;
using System.Xml.Serialization;
using System.IO;

#if !UNITY_WINRT
using System.Runtime.Serialization.Formatters.Binary;
#endif 

//We can use SharpZipLib
//or we can also use 7Zip (I prefer this one)
#if USE_SHARPZIPLIB
using ICSharpCode.SharpZipLib.GZip;
#elif USE_SEVENZIP
using SevenZip;
#endif



//This lovely little Gem comes from Pixelplacement
//with modifications by me
//
//http://pixelplacement.com/2012/03/21/simple-state-saving/
//
public class Storage
{
    const string COMPRESS_SUFFIX = "<c>";

#if UNITY_IPHONE
	static Storage()
	{
		//I want to use BinaryFormatter for serialization but 
        //at RUNTIME you'll get an error: "... Attempting to JIT compile method..."
        //Mono normally uses runtime-generated class serializers. We want to use reflection instead.
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}
#endif

    public static T Load<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(PlayerPrefs.GetString(key));
            return (T)serializer.Deserialize(sr);
        }
        else
        {
            return default(T);
        }
    }

    public static void Set<T>(string key, T source)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter sw = new StringWriter();
        serializer.Serialize(sw, source);
        PlayerPrefs.SetString(key, sw.ToString());
    }

    public static void SetB64<T>(string key, T source, bool compress = true)
    {
        byte[] bytes = null;

        bytes = Serialize<T>(source, compress);

        //If this key is compressed, add the compressed suffix to the end of it so we'll know
        if (compress)
            key += COMPRESS_SUFFIX;

        PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes));
    }

    public static bool Delete(string key)
    {
        if (PlayerPrefs.HasKey(key + COMPRESS_SUFFIX))
        {
            PlayerPrefs.DeleteKey(key + COMPRESS_SUFFIX);
            return true;
        }
        else if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            return true;
        }
        return false;
    }

    public static T GetB64<T>(string key) where T : class
    {
        if (PlayerPrefs.HasKey(key + COMPRESS_SUFFIX))
        {
            return DeserializeB64<T>(PlayerPrefs.GetString(key + COMPRESS_SUFFIX), true);
        }
        else if (PlayerPrefs.HasKey(key))
        {
            return DeserializeB64<T>(PlayerPrefs.GetString(key), false);
        }
        else
        {
            return null;
        }
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(key + COMPRESS_SUFFIX);
    }

    public static T DeserializeB64<T>(string input, bool isCompressed) where T : class
    {
        try
        {
            //Grab the bytes
            byte[] bytes = System.Convert.FromBase64String(input);

            T obj = Deserialize<T>(bytes, isCompressed);
            return obj;
        }
        catch (System.Exception)
        {
            //Dbg.Err("DeserializeB64: " + e.Message);
            return null;
        }
    }

    public static T Deserialize<T>(byte[] bytes, bool isCompressed) where T : class
    {
        try
        {
            if (isCompressed && bytes != null && bytes.Length > 1)
                bytes = Decompress(bytes);

#if !UNITY_WINRT
            using (MemoryStream m = new MemoryStream(bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                T obj = (T)bf.Deserialize(m);
                return obj;
            }
#else
            T obj = WinStore.Storage.Deserialize<T>(bytes);
            return obj;
#endif
        }
        catch (System.Exception)
        {
            //Dbg.Err("Deserialize: " + e.Message);
            return null;
        }
    }

    public static byte[] Serialize<T>(T source, bool compress)
    {
        byte[] bytes = null;
#if !UNITY_WINRT
        using (MemoryStream m = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(m, source);
            bytes = m.GetBuffer();
        }
#else
        bytes = WinStore.Storage.Serialize<T>(source);
#endif
        if (compress && bytes != null && bytes.Length > 1)
            bytes = Compress(bytes);
        return bytes;
    }

    public static byte[] Compress(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;

#if USE_SEVENZIP
            bytes = SevenZipHelper.Compress(bytes);
#elif USE_SHARPZIPLIB
        using (var ms = new MemoryStream(bytes.Length))
        {
            using (var gzo = new GZipOutputStream(ms))
            {
                gzo.IsStreamOwner = false;
                gzo.Write(bytes, 0, bytes.Length);
            }
            bytes = ms.ToArray();
        }
#endif
        return bytes;
    }

    public static byte[] Decompress(byte[] bytes)
    {
#if USE_SEVENZIP && !UNITY_WP8
        //Decompress the bytes if the data was compressed
        bytes = SevenZipHelper.Decompress(bytes);
#elif USE_SHARPZIPLIB && !UNITY_WP8
        byte[] outBytes = new byte[10];
        int size = 0;
        using (var ms = new MemoryStream(bytes, false))
        {
            var decomp = new GZipInputStream(ms);
            {
                using (var outmem = new MemoryStream(bytes.Length))
                {
                    while (true)
                    {
                        size = decomp.Read(outBytes, 0, outBytes.Length);
                        if (size > 0)
                            outmem.Write(outBytes, 0, size);
                        else
                            break;
                    }
                    bytes = outmem.ToArray();
                }
            }
        }
#endif
        return bytes;
    }

    /// <summary>
    /// This creates a copy of the given object and returns the copy
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(T other) where T: class
    {
        byte[] bytes = Serialize<T>(other, false);
        
        //Were we able to serialize?
        if (bytes == null) return null;

        return Deserialize<T>(bytes, false);
    }
}
