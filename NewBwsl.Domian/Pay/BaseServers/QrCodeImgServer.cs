using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace Pay
{
    class QrCodeImgServer
    {
        public string ticket { get; set; }
        public int scale { get; set; }
        public QrCodeImgServer(string _ticket, int _scale = 8)
        {
            this.ticket = _ticket;
            this.scale = _scale;
        }
        private byte[] Create()
        {
            QRCodeEncoder code = new QRCodeEncoder();
            code.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            code.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            code.QRCodeVersion = 0;
            code.QRCodeScale = this.scale;
            //将字符串生成二维码图片
            Bitmap image = code.Encode(this.ticket, Encoding.Default);
            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            //输出二维码图片
            return ms.GetBuffer();
        }
    }
}
