using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using OpenCVFaceTracker;
using System.IO;
using UnityEngine.Networking;

namespace FaceTrackerExample
{
    /// <summary>
    /// WebCamTexture face tracker example.
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class WebCamTextureFaceTrackerExample : MonoBehaviour
    {

        /// <summary>
        /// The auto reset mode. if ture, Only if face is detected in each frame, face is tracked.
        /// </summary>
        public bool isAutoResetMode;

        /// <summary>
        /// The gray mat.
        /// </summary>
        Mat grayMat;

        /// <summary>
        /// The texture.
        /// </summary>
        // 사용자의 얼굴을 저장할 texture
        Texture2D texture;

        /// <summary>
        /// The cascade.
        /// </summary>
        CascadeClassifier cascade;

        /// <summary>
        /// The face tracker.
        /// </summary>
        FaceTracker faceTracker;

        /// <summary>
        /// The face tracker parameters.
        /// </summary>
        FaceTrackerParams faceTrackerParams;

        /// <summary>
        /// The web cam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The tracker_model_json_filepath.
        /// </summary>
        private string tracker_model_json_filepath;

        /// <summary>
        /// The haarcascade_frontalface_alt_xml_filepath.
        /// </summary>
        private string haarcascade_frontalface_alt_xml_filepath;

        // 사진촬형 boolean
        private bool isShot = false;
        private bool isTake = false;

        // 가로, 세로 스케일 저장
        private float widthScale;
        private float heightScale;

        // 다시 찍어달라는 메시지
        public GameObject Message;
        public GameObject ShotMessage;

        public GameObject Loading;

        private bool isCamera = true;
        private bool isUpload = true;

        // Use this for initialization
        void Start()
        {

#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(getFilePathCoroutine());
#else
            tracker_model_json_filepath = Utils.getFilePath("tracker_model.json");
            haarcascade_frontalface_alt_xml_filepath = Utils.getFilePath("haarcascade_frontalface_alt.xml");
            Run();
#endif
            //Debug.Log("Age: " + MainControl.pAge);
            //Debug.Log("Gender: " + MainControl.pGender);
            //Debug.Log("Price: " + MainControl.pPrice);

            // 셀프 카메라로 바꾸고 얼굴 인식 시작
            OnChangeCameraButton();
            OnTakePicture();
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator getFilePathCoroutine()
        {
            var getFilePathAsync_0_Coroutine = StartCoroutine(Utils.getFilePathAsync("tracker_model.json", (result) => {
                tracker_model_json_filepath = result;
            }));
            var getFilePathAsync_1_Coroutine = StartCoroutine(Utils.getFilePathAsync("haarcascade_frontalface_alt.xml", (result) => {
                haarcascade_frontalface_alt_xml_filepath = result;
            }));
            
            
            yield return getFilePathAsync_0_Coroutine;
            yield return getFilePathAsync_1_Coroutine;
            
            Run();
        }
#endif

        private void Run()
        {
            //initialize FaceTracker
            faceTracker = new FaceTracker(tracker_model_json_filepath);
            //initialize FaceTrackerParams
            faceTrackerParams = new FaceTrackerParams();

            cascade = new CascadeClassifier();
            cascade.load(haarcascade_frontalface_alt_xml_filepath);
            //            if (cascade.empty())
            //            {
            //                Debug.LogError("cascade file is not loaded.Please copy from “FaceTrackerExample/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
            //            }

            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            webCamTextureToMatHelper.Init();

        }

        /// <summary>
        /// Raises the web cam texture to mat helper inited event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInited()
        {
            Debug.Log("OnWebCamTextureToMatHelperInited");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            // texture에 사용자의 얼굴을 2D texture 형태로 저장
            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = 0;
            float height = 0;

            width = gameObject.transform.localScale.x;
            height = gameObject.transform.localScale.y;

            // 화면 스케일 저장
            widthScale = (float)Screen.width / width;
            heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            } else
            {
                Camera.main.orthographicSize = height / 2;
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            grayMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);


        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            faceTracker.reset();
            grayMat.Dispose();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(Input.mousePosition);
            }

            if (isCamera)
            {
                if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
                {

                    Mat rgbaMat = webCamTextureToMatHelper.GetMat();

                    //convert image to greyscale
                    Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);

                    if (isAutoResetMode || faceTracker.getPoints().Count <= 0)
                    {
                        //                                      Debug.Log ("detectFace");

                        //convert image to greyscale
                        using (Mat equalizeHistMat = new Mat())
                        using (MatOfRect faces = new MatOfRect())
                        {

                            Imgproc.equalizeHist(grayMat, equalizeHistMat);

                            cascade.detectMultiScale(equalizeHistMat, faces, 1.1f, 2, 0
                                //                                                                                 | Objdetect.CASCADE_FIND_BIGGEST_OBJECT
                                | Objdetect.CASCADE_SCALE_IMAGE, new OpenCVForUnity.Size(equalizeHistMat.cols() * 0.15, equalizeHistMat.cols() * 0.15), new Size());


                            if (faces.rows() > 0)
                            {
                                //                                              Debug.Log ("faces " + faces.dump ());

                                List<OpenCVForUnity.Rect> rectsList = faces.toList();
                                List<Point[]> pointsList = faceTracker.getPoints();

                                if (isAutoResetMode)
                                {
                                    //add initial face points from MatOfRect
                                    if (pointsList.Count <= 0)
                                    {
                                        faceTracker.addPoints(faces);
                                        //                                                                      Debug.Log ("reset faces ");
                                    }
                                    else
                                    {
                                        // 얼굴 부분을 찍어야할 때
                                        if (isShot)
                                        {
                                            for (int i = 0; i < rectsList.Count; i++)
                                            {

                                                OpenCVForUnity.Rect trackRect = new OpenCVForUnity.Rect(rectsList[i].x + rectsList[i].width / 3, rectsList[i].y + rectsList[i].height / 2, rectsList[i].width / 3, rectsList[i].height / 3);
                                                //It determines whether nose point has been included in trackRect.                                      
                                                if (i < pointsList.Count && !trackRect.contains(pointsList[i][67]))
                                                {
                                                    rectsList.RemoveAt(i);
                                                    pointsList.RemoveAt(i);
                                                    //                                                                                      Debug.Log ("remove " + i);
                                                }

                                                double left = 0;
                                                double right = 0;
                                                double up = 0;
                                                double down = 0;

                                                //track face points.if face points <= 0, always return false.
                                                if (faceTracker.track(rgbaMat, faceTrackerParams))
                                                {
                                                    // 얼굴 좌푤르 저장할 Point List
                                                    pointsList = faceTracker.getPoints();

                                                    // 상하좌우 기준점을 찾음
                                                    left = findMin(pointsList[0][0].x, pointsList[0][1].x, pointsList[0][2].x, pointsList[0][3].x);
                                                    right = findMax(pointsList[0][14].x, pointsList[0][13].x, pointsList[0][12].x, pointsList[0][11].x);
                                                    up = findMin(pointsList[0][16].y, pointsList[0][17].y, pointsList[0][22].y, pointsList[0][23].y);
                                                    down = findMax(pointsList[0][6].y, pointsList[0][7].y, pointsList[0][8].y, pointsList[0][9].y);

                                                    // 얼굴 좌표 출력
                                                    //for (int i = 0; i < pointsList[0].Length; i++)
                                                    //{
                                                    //    Imgproc.putText(imgMat, i + "", pointsList[0][i], Core.FONT_HERSHEY_SIMPLEX, 0.2, new Scalar(255, 2, 10, 255), 1, Imgproc.LINE_AA, false);
                                                    //}

                                                    //faceTracker.draw(imgMat, new Scalar(255, 0, 0, 255), new Scalar(0, 255, 0, 255));

                                                }

                                                // 가로폭, 세로폭을 정함
                                                double widthLR = (right - left) / 10;
                                                double heightU = (down - up) / 3;
                                                double heightD = (down - up) / 18;

                                                // 스크린 상의 빨간색 네모 왼쪽 위와 오른쪽 아래의 좌표 찾음
                                                Vector2 screenPos1 = Camera.main.WorldToScreenPoint(new Vector2((float)(left - widthLR), (float)(up - heightU)));
                                                Vector2 screenPos2 = Camera.main.WorldToScreenPoint(new Vector2((float)(right + widthLR), (float)(down + heightD)));

                                                // 스크린 상의 그림의 왼쪽 위와 오른쪽 위 좌표
                                                Vector2 leftUp = Camera.main.WorldToScreenPoint(new Vector2(0f, 0f));
                                                Vector2 rightUp = Camera.main.WorldToScreenPoint(new Vector2(texture.width / 2, texture.height / 2));

                                                // 가로, 세로 여백의 크기 추출
                                                float widthSpace = Screen.width - rightUp.x;
                                                float heightSpace = Screen.height - rightUp.y;

                                                // 중앙의 스크린 좌표
                                                Vector2 mid = Camera.main.WorldToScreenPoint(new Vector2(0f, 0f));

                                                // 그림과 빨간색 상자 사이의 여백
                                                float leftSpace = screenPos1.x - leftUp.x;
                                                float upSpace = screenPos1.y - leftUp.y;

                                                // 빨간색 상자의 가로, 세로를 정함
                                                float widthImg = screenPos2.x - screenPos1.x;
                                                float heightImg = screenPos2.y - screenPos1.y;
                                                //Debug.Log("width: " + widthImg + ", height: " + heightImg);

                                                // 저장을 할 땐 사각형 표시 X   
                                                if (!isTake)
                                                {
                                                    // 기준점과 폭을 토대로 사각형을 그림
                                                    Imgproc.rectangle(rgbaMat, new Point(left - widthLR, up - heightU), new Point(right + widthLR, down + heightD), new Scalar(255, 0, 0, 255), 2);
                                                }

                                                if (isTake)
                                                {
                                                    // image를 정사각형으로 저장
                                                    // 가로가 크냐 세로가 크냐에 따라 따로 저장
                                                    if (widthImg > heightImg)
                                                    {
                                                        int picX = (int)(widthSpace + leftSpace);
                                                        int picY = (int)(Screen.height - upSpace - heightImg - (widthImg - heightImg) / 2 - heightSpace);
                                                        
                                                        // 얼굴 크기가 화면 크기 1/4 보다 커야하고 얼굴이 중앙
                                                        if (widthImg > Screen.width / 4 && picX < mid.x && picX + widthImg > mid.x && picY < mid.y && picY + heightImg > mid.y)
                                                            StartCoroutine(downImage((int)widthImg, picX, picY));
                                                        else
                                                            StartCoroutine(reShot());
                                                    }
                                                    else
                                                    {
                                                        int picX = (int)(widthSpace + leftSpace - (heightImg - widthImg) / 2);
                                                        int picY = (int)(Screen.height - upSpace - heightImg - heightSpace);

                                                        //Debug.Log("picX : " + picX);
                                                        //Debug.Log("picY : " + picY);
                                                        //Debug.Log("wid : " + widthImg);
                                                        //Debug.Log("hei : " + heightImg);
                                                        //Debug.Log("midX : " + mid.x);
                                                        //Debug.Log("midY : " + mid.y);

                                                        if (heightImg > Screen.width / 4 && picX < mid.x && picX + widthImg > mid.x && picY < mid.y && picY + heightImg > mid.y)
                                                            StartCoroutine(downImage((int)heightImg, picX, picY));
                                                        else
                                                            StartCoroutine(reShot());
                                                    }

                                                    // 안쪽 파란 사각형으로 얼굴 표현
                                                    //Imgproc.rectangle(rgbaMat, new Point(trackRect.x, trackRect.y), new Point(trackRect.x + trackRect.width, trackRect.y + trackRect.height), new Scalar(0, 0, 255, 255), 2);
                                                }

                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    faceTracker.addPoints(faces);
                                }

                            }
                            else
                            {
                                if (isTake)
                                    StartCoroutine(reShot());

                                if (isAutoResetMode)
                                {
                                    faceTracker.reset();
                                }
                            }
                        }

                    }

                    // 얼굴 부분을 점으로 표현
                    //track face points.if face points <= 0, always return false.
                    //if (faceTracker.track(grayMat, faceTrackerParams))
                    //    faceTracker.draw(rgbaMat, new Scalar(255, 0, 0, 255), new Scalar(0, 255, 0, 255));


#if OPENCV_2
                Core.putText (rgbaMat, "'Tap' or 'Space Key' to Reset", new Point (5, rgbaMat.rows () - 5), Core.FONT_HERSHEY_SIMPLEX, 0.8, new Scalar (255, 255, 255, 255), 2, Core.LINE_AA, false);
#else
                    //Imgproc.putText(rgbaMat, "'Tap' or 'Space Key' to Reset", new Point(5, rgbaMat.rows() - 5), Core.FONT_HERSHEY_SIMPLEX, 0.8, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
#endif


                    //                              Core.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Core.LINE_AA, false);

                    Utils.matToTexture2D(rgbaMat, texture, webCamTextureToMatHelper.GetBufferColors());

                }
            }

        }

        // 최솟값을 찾아내는 함수
        private double findMin(double num1, double num2, double num3, double num4)
        {
            double min = num1;
            if (num2 < min)
                min = num2;
            if (num3 < min)
                min = num3;
            if (num4 < min)
                min = num4;

            return min;
        }

        // 최댓값을 찾아내는 함수
        private double findMax(params double[] nums)
        {
            double max = nums[0];

            for (int i=1; i<nums.Length; i++)
            {
                if (max < nums[i]) max = nums[i];
            }

            return max;
        }

        // 2초 후 사진촬영 함수
        IEnumerator shotPlayer()
        {

            isShot = true;

            ShotMessage.SetActive(true);

            // 2초동안 기다림
            yield return new WaitForSeconds(3.5f);

            ShotMessage.SetActive(false);

            isTake = true;

        }

        // 다시 사진 찍어달라고 메시지 표시
        IEnumerator reShot()
        {
            Message.SetActive(true);

            // 1 초후에 메시지 종료
            yield return new WaitForSeconds(1);

            Message.SetActive(false);

            isShot = false;
            isTake = false;

            // 다시 얼굴 인식
            OnTakePicture();
        }

        // image Download Function
        IEnumerator downImage(int sizeImg, int startX, int startY)
        {
            yield return new WaitForEndOfFrame();

            // 자르는 크기 토대로 2D Texture 생성
            Texture2D tex = new Texture2D(sizeImg, sizeImg, TextureFormat.RGB24, false);
            //Debug.Log("SX: " + startX + ", SY: " + startY + ", Size: " + sizeImg);

            // Read screen contents into the texture
            tex.ReadPixels(new UnityEngine.Rect(startX, startY, sizeImg, sizeImg), 0, 0);
            tex.Apply();

            //tex.Resize(300, 300);

            // Encode texture into PNG
            byte[] bytes = ScaleTexture(tex).EncodeToJPG();
            UnityEngine.Object.Destroy(tex);

            // newImg를 JPG로 바꾸어 저장
            //File.WriteAllBytes("Assets/New/face.jpg", bytes);
            //Debug.Log("OK");

            // 0.5초 후에 씬 넘어감
            yield return new WaitForSeconds(0.5f);

            // 저장이 끝났으면 촬영을 멈추고 다음 씬으로 이동
            isShot = false;
            isTake = false;
            isCamera = false;
            
            Loading.SetActive(true);
            StartCoroutine(UploadFileCo(bytes));

        }

        private const string URL = "http://52.196.87.0/send";
        private const string EXTENSION = ".jpg";

        // 이미지 업로드
        IEnumerator UploadFileCo(byte[] binary)
        {
            if(isUpload)
            {
                isUpload = false;

                string fileName = "abcd";

                WWWForm form = new WWWForm();
                form.AddBinaryData("userfile", binary, fileName);

                WWW upload = new WWW(URL, form);
                yield return upload;

                if (string.IsNullOrEmpty(upload.error))
                {

                    try
                    {
                        Recommend.Response = JsonUtility.FromJson<Recommend.JsonResponse>(upload.text);
                        Recommend.selMap = new int[3];
                        for (int i = 0; i < 3; i++)
                        {
                            Recommend.selMap[i] = Random.Range(0, 7);
                            for (int j = 0; j < i; j++)
                            {
                                if (Recommend.selMap[i] == Recommend.selMap[j])
                                {
                                    i--;
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Exception Ocurred: ");
                        Debug.Log(e);
                    }
                    Debug.Log("upload done :" + upload.text);

                    SceneManager.LoadScene("RecommendGlasses");
                }
                else
                {
                    Debug.Log("Error during upload: " + upload.error);
                }
            }
        }

        // 사진 크기 조정
        private Texture2D ScaleTexture(Texture2D source)
        {
            int targetWidth = 224;
            int targetHeight = 224;
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
            webCamTextureToMatHelper.Dispose();

            if (cascade != null)
                cascade.Dispose();
        }

        /// <summary>
        /// Raises the change camera button event.
        /// </summary>
        public void OnChangeCameraButton()
        {
            webCamTextureToMatHelper.Init(null, webCamTextureToMatHelper.requestWidth, webCamTextureToMatHelper.requestHeight, !webCamTextureToMatHelper.requestIsFrontFacing);
        }

        public void OnTakePicture()
        {
            // 촬영하고 있지 않을 때
            if(!isShot)
                StartCoroutine(shotPlayer());
        }
                
    }
}