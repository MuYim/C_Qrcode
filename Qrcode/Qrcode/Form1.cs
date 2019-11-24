using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


using ThoughtWorks.QRCode.Codec;

namespace Qrcode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


 //--------------------生成二维码并返回二维码保存地址-----------------------
        string codeUrl="";//生成二维码存放地址
        string grimg = "", wzimg = "", wyimg = "";//名片，文字，网页另存为时使用的地址
        //默认logo
        string imglogo = AppDomain.CurrentDomain.BaseDirectory + "Qrcode\\logo.png";
        private string CreateQrcode(string content,string error)
        {
            int border = 10;//设置二维码的边框大小
            int size = 330;//设置二维码的大小
            System.Drawing.Bitmap bt;
            string enCodeString = content;

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            qrCodeEncoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            qrCodeEncoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            //错误效验、错误更正(有4个等级)
            if (error == "L")
            {
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            }
            else if (error == "M")
            {
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            }
            else if (error == "Q")
            {
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            }
            else if (error == "H")
            {
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            }
            qrCodeEncoder.QRCodeBackgroundColor = Color.White;//背景色
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;//前景色

            bt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);

            //当设定目标图片尺寸大于生成的尺寸时，逐步增大方格尺寸
            #region 当设定目标图片尺寸大于生成的尺寸时，逐步增大方格尺寸
            while (bt.Width < size)
            {
                qrCodeEncoder.QRCodeScale++;
                System.Drawing.Bitmap imageNew = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                if (imageNew.Width < size)
                {
                    bt = new System.Drawing.Bitmap(imageNew);
                    imageNew.Dispose();
                    imageNew = null;
                }
                else
                {
                    qrCodeEncoder.QRCodeScale--; //新尺寸未采用，恢复最终使用的尺寸
                    imageNew.Dispose();
                    imageNew = null;
                    break;
                }
            }
            #endregion

            //当设定目标图片尺寸小于生成的尺寸时，逐步减小方格尺寸
            #region 当设定目标图片尺寸小于生成的尺寸时，逐步减小方格尺寸
            while (bt.Width > size && qrCodeEncoder.QRCodeScale > 1)
            {
                qrCodeEncoder.QRCodeScale--;
                System.Drawing.Bitmap imageNew = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                bt = new System.Drawing.Bitmap(imageNew);
                imageNew.Dispose();
                imageNew = null;
                if (bt.Width < size)
                {
                    break;
                }
            }
            #endregion

            //如果目标尺寸大于生成的图片尺寸，则为图片增加白边
            #region 如果目标尺寸大于生成的图片尺寸，则为图片增加白边
            if (bt.Width <= size)
            {
                //根据参数设置二维码图片白边的最小宽度
                #region 根据参数设置二维码图片白边的最小宽度
                if (border > 0)
                {
                    while (bt.Width <= size && size - bt.Width < border * 2 && qrCodeEncoder.QRCodeScale > 1)
                    {
                        qrCodeEncoder.QRCodeScale--;
                        System.Drawing.Bitmap imageNew = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                        bt = new System.Drawing.Bitmap(imageNew);
                        imageNew.Dispose();
                        imageNew = null;
                    }
                }
                #endregion

                //当目标图片尺寸大于二维码尺寸时，将二维码绘制在目标尺寸白色画布的中心位置
                if (bt.Width < size)
                {
                    //新建空白绘图
                    System.Drawing.Bitmap panel = new System.Drawing.Bitmap(size, size);
                    System.Drawing.Graphics graphic0 = System.Drawing.Graphics.FromImage(panel);
                    int p_left = 0;
                    int p_top = 0;
                    if (bt.Width <= size) //如果原图比目标形状宽
                    {
                        p_left = (size - bt.Width) / 2;
                    }
                    if (bt.Height <= size)
                    {
                        p_top = (size - bt.Height) / 2;
                    }

                    //将生成的二维码图像粘贴至绘图的中心位置
                    graphic0.DrawImage(bt, p_left, p_top, bt.Width, bt.Height);
                    bt = new System.Drawing.Bitmap(panel);
                    panel.Dispose();
                    panel = null;
                    graphic0.Dispose();
                    graphic0 = null;
                }
            }
            #endregion

            //二维码保存
            Random rd = new Random();
            int i = rd.Next();
            string filename = i.ToString();//再多次生成的生成二维码的过程中防止重名报错
            string file_path = AppDomain.CurrentDomain.BaseDirectory + "Qrcode\\";
            codeUrl = file_path + filename + ".jpg";
            //根据文件名称，自动建立对应目录
            if (!System.IO.Directory.Exists(file_path))
                System.IO.Directory.CreateDirectory(file_path);
            bt.Save(codeUrl);//保存图片
            return codeUrl;
            //pictureBox2.Image = Image.FromFile(codeUrl);
            //pictureBox2.Image.Dispose();
        }
//--------------------生成二维码并返回二维码保存地址---结束--------------------

//-----------------------------二维码添加logo----------------------------------

        /// <summary>    
         /// 调用此函数后使此两种图片合并，类似相册，有个    
         /// 背景图，中间贴自己的目标图片    
         /// </summary>    
         /// <param name="imgBack">粘贴的源图片</param>    
         /// <param name="destImg">粘贴的目标图片</param>    
         public string CombinImage(string imgBack1, string destImg)
         {
             Image img = Image.FromFile(destImg);        //照片图片      
             Image imgBack = Image.FromFile(imgBack1);        //照片图片     
             if (img.Height != 90 || img.Width != 90)
             {
                 img = KiResizeImage(img, 90, 90, 0);
             }
             Graphics g = Graphics.FromImage(imgBack);
 
             g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);     
 
             //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);
             //相片四周刷一层黑色边框    
             //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    
 
             g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
             GC.Collect();
             //二维码保存
             Random rd = new Random();
             int i = rd.Next();
             string filename = i.ToString();//再多次生成的生成二维码的过程中防止重名报错
             string file_path = AppDomain.CurrentDomain.BaseDirectory + "Qrcode\\";
             string codelogoUrl = file_path + filename + ".png";
             //根据文件名称，自动建立对应目录
             if (!System.IO.Directory.Exists(file_path))
                 System.IO.Directory.CreateDirectory(file_path);
             imgBack.Save(codelogoUrl);//保存图片 
             grimg = codelogoUrl;
             return codelogoUrl;
         }
 
 
         /// <summary>    
         /// Resize图片    
         /// </summary>    
         /// <param name="bmp">原始Bitmap</param>    
         /// <param name="newW">新的宽度</param>    
         /// <param name="newH">新的高度</param>    
         /// <param name="Mode">保留着，暂时未用</param>    
         /// <returns>处理以后的图片</returns>    
         public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
         {
             try
             {
                 Image b = new Bitmap(newW, newH);
                 Graphics g = Graphics.FromImage(b);
                 // 插值算法的质量    
               //  g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                 g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                 g.Dispose();
                 return b;
             }
             catch
             {
                 return null;
             }
         }

//-----------------------------二维码添加logo----------------------------------

//-----------------------------二维码另存为--------开始--------------------------

         private void ShowSaveFileDialog(string image)
         {
             if (image != "")
             {
                 Image img = Image.FromFile(image);  
                 //string localFilePath, fileNameExt, newFileName, FilePath; 
                 SaveFileDialog sfd = new SaveFileDialog();

                 //设置文件类型 
                 sfd.Filter = "Jpg 图片|*.jpg|Bmp 图片|*.bmp|Gif 图片|*.gif|Png 图片|*.png|Wmf  图片|*.wmf";

                 //设置默认文件类型显示顺序 
                 sfd.FilterIndex = 1;

                 //保存对话框是否记忆上次打开的目录 
                 sfd.RestoreDirectory = true;

                 sfd.CheckPathExists = true;//检查目录

                 //设置默认的文件名

                 sfd.FileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "-"; ;//设置默认文件名

                 //点了保存按钮进入 
                 /*if (sfd.ShowDialog() == DialogResult.OK)
                 {
                     string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                     string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                 }*/
                 if (sfd.ShowDialog() == DialogResult.OK)
                 {
                     img.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);// image为要保存的图片
                     MessageBox.Show(this, "图片保存成功！", "信息提示");
                 }
             }
             else
                 MessageBox.Show(this, "图片保存失败！", "信息提示");
         }

//--------------------二维码另存为---结束----------------------------------------

//--------------------个人名片---开始------------------------------------------

        //logo上传
         private void button4_Click(object sender, EventArgs e)
         {
             if (openFileDialog1.ShowDialog() == DialogResult.OK)
             {
                 
                 //获取用户选择文件的后缀名 
                   string extension = Path.GetExtension(openFileDialog1.FileName);
                   //声明允许的后缀名 
                   string[] str = new string[] { ".gif", ".jpeg", ".jpg", ".png" };
                   if (!str.Contains(extension))
                   {
                       MessageBox.Show("仅能上传gif,jpeg,jpg格式的图片！");
                   }
                   else
                   {
                       //获取用户选择的文件，并判断文件大小不能超过20K，fileInfo.Length是以字节为单位的 
                       FileInfo fileInfo = new FileInfo(openFileDialog1.FileName);
                       if (fileInfo.Length > 4096000)
                       {
                           MessageBox.Show("上传的图片不能大于4M");
                       }
                       else
                       {
                           //PictureBox控件显示图片
                           pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                           //绝对路径
                           string image = openFileDialog1.FileName;
                           //添加随机部分防止重名
                           Random rd = new Random();
                           int i = rd.Next();
                           string filename = i.ToString();//再多次生成的生成二维码的过程中防止重名报错
                           string picpath = openFileDialog1.SafeFileName;
                           imglogo = Application.StartupPath + "\\Qrcode\\" + filename + picpath;
                           File.Copy(openFileDialog1.FileName, Application.StartupPath + "\\Qrcode\\" +filename+picpath); 
                       }
                   }
                   
             }
         }
        //名片生成
        private void button1_Click(object sender, EventArgs e)
        {
           //获取个人名片的内容
            string name = textBox1.Text;
            string company = textBox2.Text;
            string position = textBox3.Text;
            string phone = textBox4.Text;
            string email = textBox5.Text;
            string address = textBox6.Text;
            string content = "BEGIN:VCARD\nFN:" + name + "\nORG:" + company + "\nTITLE:"
                + position + "\nTEL:" + phone + "\nADR:" + address + "\nEMAIL:"
                + email + "\nEND:VCARD";
            //生成二维码
            CreateQrcode(content,"M");
            //CombinImage(codeUrl, imglogo);
            pictureBox2.Image = Image.FromFile( CombinImage(codeUrl, imglogo));
           
        }
        //清空按钮
        private void button2_Click(object sender, EventArgs e)
        {   //置空文字输入
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;
            //置空生成的二维码
            if (pictureBox2.Image != null)
            {
                Image image = pictureBox2.Image;
                pictureBox2.Image = null;
                image.Dispose();
            }
            
            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ShowSaveFileDialog(grimg);
        }
//--------------------个人名片---结束------------------------------------------


//--------------------文字转换---开始------------------------------------------
        private void button5_Click(object sender, EventArgs e)
        {
            //获取文字转换的内容
            string content = textBox7.Text;
            wzimg = CreateQrcode(content,"M");
            //生成二维码
            pictureBox3.Image = Image.FromFile(wzimg);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //置空文字输入
            textBox7.Text = string.Empty;
            //置空生成的二维码
            if (pictureBox3.Image != null)
            {
                Image image = pictureBox3.Image;
                pictureBox3.Image = null;
                image.Dispose();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowSaveFileDialog(wzimg);
        }

       
//--------------------文字转换---结束------------------------------------------

//--------------------网页跳转---开始------------------------------------------

        private void button8_Click(object sender, EventArgs e)
        {
            //获取网址
            string content = "http://"+textBox8.Text;
            //生成二维码
            wyimg = CreateQrcode(content,"M");
            pictureBox4.Image = Image.FromFile(wyimg);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //置空文字输入
            textBox8.Text = string.Empty;
            //置空生成的二维码
            if (pictureBox4.Image != null)
            {
                Image image = pictureBox4.Image;
                pictureBox4.Image = null;
                image.Dispose();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowSaveFileDialog(wyimg);
        }

       

//--------------------网页跳转---结束------------------------------------------
//--------------------纠错演示---开始------------------------------------------

        private void button11_Click(object sender, EventArgs e)
        {
            //获取测试内容
            string content =textBox9.Text;
            //生成二维码
            string error=comboBox1.Text;
            pictureBox5.Image = Image.FromFile(CreateQrcode(content, error));
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //置空文字输入
            textBox9.Text = string.Empty;
            //置空生成的二维码
            if (pictureBox5.Image != null)
            {
                Image image = pictureBox5.Image;
                pictureBox5.Image = null;
                image.Dispose();
            }
            //恢复遮挡的初始位置和大小
            pictureBox6.Left = 642;
            pictureBox6.Top = 389;
            pictureBox6.Width = 90;
            pictureBox6.Height = 90;
        }

//===========================================================================
        Point orignalPoint = new Point();

        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            orignalPoint = e.Location;//记录鼠标按下时的坐标
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pictureBox6.Left += e.X - orignalPoint.X;//当前点的横坐标减鼠标按下时的横坐标
                pictureBox6.Top += e.Y - orignalPoint.Y;//当前点的纵坐标减鼠标按下时的纵坐标
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            pictureBox6.Width += 10;
            pictureBox6.Height += 10;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            pictureBox6.Width -= 10;
            pictureBox6.Height -= 10;
        }
        


//--------------------纠错演示---结束------------------------------------------
  
    }
}
