using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian
{
    public class ImageHelper
    {
        /// <summary>
        /// 将图片读取为byte[]
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public byte[] GetPictureData(string imagePath)
        {
            using (FileStream fs = new FileStream(imagePath, FileMode.Open))
            {
                byte[] byteData = new byte[fs.Length];
                fs.Read(byteData, 0, byteData.Length);
                fs.Close();
                return byteData;
            }
        }
    }
}
