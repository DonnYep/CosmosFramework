using Cosmos.DataTable;
using Cosmos;
using System.Text;
using System;

public class SampleCSVDataRow : IDataRow
{
    SampleCSV sampleCSV = new SampleCSV();
    public int Id { get { return sampleCSV.Id; } }
    public SampleCSV SampleCSV { get { return sampleCSV; } }
    public bool ParseData(byte[] dataBytes)
    {
        if (dataBytes != null)
        {
            var dataString = Encoding.UTF8.GetString(dataBytes);
            return ParseData(dataString);
        }
        else
            return false;
    }
    public bool ParseData(string dataString)
    {
        try
        {
            var strArr = dataString.Split(',');
            var type = typeof(SampleCSV);
            var map = Utility.Assembly.GetTypeFieldsNameAndTypeMapping(type);
            int idx = 0;
            foreach (var e in map)
            {
                if (ParseStringToBaseType(strArr[idx], out var rst))
                {
                    Utility.Assembly.SetFieldValue(type, sampleCSV, e.Key, rst);
                }
                else
                {
                    Utility.Assembly.SetFieldValue(type, sampleCSV, e.Key, strArr[idx]);
                }
                idx++;
            }
            return true;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    bool ParseStringToBaseType(string str, out object rst)
    {
        rst = null;
        if (bool.TryParse(str, out var boolRst))
        {
            rst = boolRst;
            return true; ;
        }
        if (int.TryParse(str, out var longRst))
        {
            rst = longRst;
            return true;
        }
        if (float.TryParse(str, out var floatRst))
        {
            rst = floatRst;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        sampleCSV.Reset();
    }
}
