using System.IO;


namespace ZEditor
{
    class BuildNo
    {
        public static int Get()
        {
            string path = "buildno.txt";
            if (!File.Exists(path))
            {
                var sw = File.CreateText(path);
                sw.Write(1);
                sw.Close();
                return 1;
            }
            string text = File.ReadAllText(path);
            int result;
            if (!int.TryParse(text, out result))
            {
                result = 1;
            }
            return result;
        }

        public static int Inc()
        {
            string path = "buildno.txt";
            string text = File.ReadAllText(path);
            int result;
            if (!int.TryParse(text, out result))
            {
                result = 1;
            }
            result += 1;
            using (var w = File.CreateText(path))
            {
                w.Write(result);
            }
            return result;
        }
    }
}
