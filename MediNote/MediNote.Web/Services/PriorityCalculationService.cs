namespace MediNote.Web.Services
{
    /// Author: Bilal Ahmed Samoon
    /// Provides business logic for calculating appointment priority based on symptoms.
    public class PriorityCalculationService
    {
        public string GetPriority(string symptoms)
        {
            if (string.IsNullOrWhiteSpace(symptoms))
            {
                return "LOW";
            }

            string normalized = symptoms.ToLower();

            if (normalized.Contains("chest") ||
                normalized.Contains("heart") ||
                normalized.Contains("breathing"))
            {
                return "HIGH";
            }

            if (normalized.Contains("fever") ||
                normalized.Contains("infection") ||
                normalized.Contains("pain"))
            {
                return "MEDIUM";
            }

            return "LOW";
        }
    }
}