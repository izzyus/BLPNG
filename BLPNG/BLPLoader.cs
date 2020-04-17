using System.Drawing;
using System.IO;
using SereniaBLPLib;

namespace FileLoader
{
    public class BLPLoader
    {
        public Bitmap bmp;
        public void LoadBLP(string filename)
        {
            using (FileStream reader = File.OpenRead(filename))
            {
                var blp = new BlpFile(reader);
                {
                    bmp = blp.GetBitmap(0);
                }
                reader.Close();
            }
        }
    }
}