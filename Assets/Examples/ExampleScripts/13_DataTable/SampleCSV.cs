/// <summary>
/// 一行配置的纯数据容器（示例用）。
/// <para>行对象加载后只读共享、不回收复用，因此不再需要 Reset()。</para>
/// </summary>
public class SampleCSV
{
    public int Id;
    public string Desc;
    public string Name;
    public int JobNumber;
    public float AmountOfSales;
    public string CompanyName;
    public override string ToString()
    {
        return $"Id: {Id}\n,Desc: {Desc}\n,Name: {Name}\n,JobNumber: {JobNumber}\n,AmountOfSales: {AmountOfSales}\n,CompanyName: {CompanyName}\n";
    }
}
