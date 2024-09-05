namespace SubspaceStats.Models
{
    public struct PagingInfo
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public bool HasMore { get; set; }
    }
}
