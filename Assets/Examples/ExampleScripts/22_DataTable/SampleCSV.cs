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
    public void Reset()
    {
        Id = 0;
        Desc = string.Empty;
        Name = string.Empty;
        JobNumber = 0;
        AmountOfSales = 0;
        CompanyName = string.Empty;
    }
}
