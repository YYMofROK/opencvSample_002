using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opencv_002
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        VideoCapture videoCapture;

        private void Form1_Load(object sender, EventArgs e)
        {

            for(int deviceNum=1; 99999 > deviceNum; deviceNum++)
            {
                videoCapture = new VideoCapture(deviceNum);
                if (videoCapture.IsOpened() == true)
                {
                    log_write("카메라 연결 성공 DeviceID : " + Convert.ToString(deviceNum));
                    break;
                }
            }

            videoCapture.Set(CaptureProperty.FrameWidth, 640);
            videoCapture.Set(CaptureProperty.FrameHeight, 480);

            log_write("videoCapture.FrameWidth:" + Convert.ToString(videoCapture.FrameWidth));
            log_write("videoCapture.FrameHeight:" + Convert.ToString(videoCapture.FrameHeight));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            log_1.Text = null;
            log_write("START BUTTON CLICK");

            timer1.Stop();
            timer1.Interval = 50; //스케쥴 간격 설정
            timer1.Start(); //타이머를 가동시킨다.
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (videoCapture.IsOpened() == false)
            {
                log_write("카메라 연결 안됨");
            }

            Mat frame_img_source_01 = new Mat();
            videoCapture.Read(frame_img_source_01);
            
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame_img_source_01);

            // 그레이 스케일
            Mat frame_img_source_02 = new Mat();
            videoCapture.Read(frame_img_source_02);
            Mat frame_img_gray = new Mat();
            byte[] imageBytes = frame_img_source_02.ToBytes(".bmp");
            //byte[] imageBytes = frame_img_FlipY.ToBytes(".bmp");
            frame_img_gray = Mat.FromImageData(imageBytes, ImreadModes.Grayscale);
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame_img_gray);

            Cv2.EqualizeHist(frame_img_gray, frame_img_gray);
            var cascade = new CascadeClassifier("C://Users//dev-yym//source//repos//opencv_002//FaceML_Data//haarcascade_frontalface_alt.xml");
            //var nestedCascade = new CascadeClassifier("C://Users//dev-yym//source//repos//opencv_002//FaceML_Data//haarcascade_eye_tree_eyeglasses.xml");


            var faces = cascade.DetectMultiScale(
                image: frame_img_gray,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                minSize: new OpenCvSharp.Size(30, 30)
                );

            log_write("Detected faces:" + Convert.ToString(faces.Length));


            for(int faceCnt=0; faceCnt < faces.Length; faceCnt++)
            {

                log_write("faces:" + Convert.ToString(faces[faceCnt]));

                Cv2.Rectangle(frame_img_source_01, faces[faceCnt], Scalar.YellowGreen, 2);

            }
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame_img_source_01);
        }

        private void log_write(String logText)
        {
            log_1.Text = Convert.ToString(DateTime.Now) + "-" +logText + "\r\n" + log_1.Text;

        }


    }
}
