public class GCodeCommand
{
    public string CommandType { get; set; } // G0, G1, G28
    public int Code { get; set; }
    public double X { get; set; } = double.NaN;
    public double Y { get; set; } = double.NaN;
    public double Z { get; set; } = double.NaN;
    public double FeedRate { get; set; } = double.NaN;
}
