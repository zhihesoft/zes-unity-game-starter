using System.IO;


namespace Zes
{
    public static class BuildNo
    {
        const string buildNoPath = "buildno.txt";

        public static int Get()
        {
            if (!File.Exists(buildNoPath))
            {
                return 0;
            }
            string text = File.ReadAllText(buildNoPath);
            int result;
            if (!int.TryParse(text, out result))
            {
                result = 0;
            }
            return result;
        }

        public static int Inc()
        {
            int result = Get();
            result += 1;
            using (var w = File.CreateText(buildNoPath))
            {
                w.Write(result);
            }
            return result;
        }
    }
}
