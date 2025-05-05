namespace KeepTrackApp.Models
{
    public class Supplement
    {
        public int SupplementId { get; set; }
        public string AdminId { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }        // NEW: for dosage info
        public string Instructions { get; set; }  // NEW: for how to take
        public string Benefits { get; set; }
        public string Risks { get; set; }

        public void AddSupplement() { }
        public void EditSupplement() { }
        public void DeleteSupplement() { }
    }
}
