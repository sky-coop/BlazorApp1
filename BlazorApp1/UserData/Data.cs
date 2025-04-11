using BlazorApp1.Session;
using BlazorApp1.UserData.DataParts;
using MemoryPack;
using System.Diagnostics;

namespace BlazorApp1.UserData;

[MemoryPackable]
public partial class Data
{
    public static string DataFolder => Path.Combine(AppContext.BaseDirectory, "Data");
    public static string GetDataFile(long dataID) => Path.Combine(DataFolder, $"{dataID}.mpk");

    public string Username { get; private set; } = string.Empty;
    public long DataID { get; private set; }
    public DataCounter DataCounter { get; private set; } = new();
    public string DataFile => GetDataFile(DataID);

    [MemoryPackConstructor]
    private Data() { } // 必须要有一个无参构造函数（可以是 private）
    public Data(string username, long dataID)
    {
        Username = username;
        DataID = dataID;
    }
    public static Data? Load(string? username)
    {
        if (username == null) return null;
        long? result = LoginDB.GetDataID(username);
        if (result == null) return null;
        var id = result.Value;
        try
        {
            var file = GetDataFile(id);
            if (File.Exists(file))
            {
                var bytes = File.ReadAllBytes(file);
                var loaded = MemoryPackSerializer.Deserialize<Data>(bytes);
                if (loaded != null) return loaded;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
        }
        return new(username, id);
    }
    public void Save()
    {
        try
        {
            Directory.CreateDirectory(DataFolder); // 保证文件夹存在
            var bytes = MemoryPackSerializer.Serialize(this);
            File.WriteAllBytes(DataFile, bytes);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}

