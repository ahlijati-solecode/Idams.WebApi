namespace Idams.Core.Model
{
    public static class IdGenerationUtil
    {
        public static string GenerateNextId(string? currentId, string prefix, int digit = 5)
        {
            string digitFormat = $"D{digit}";
            int currentNum;
            if (string.IsNullOrWhiteSpace(currentId))
            {
                currentNum = 0;
            }
            else
            {
                string[] ids = currentId.Split('_');
                currentNum = int.Parse(ids[2]);
            }
            currentNum++;
            string serial = currentNum.ToString(digitFormat);
            return $"{prefix}_{DateTime.UtcNow.Year}_{serial}";
        }
    }
}
