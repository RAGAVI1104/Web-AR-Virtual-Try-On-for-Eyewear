const video = document.getElementById('video');
const canvas = document.getElementById('overlay');
const context = canvas.getContext('2d');

Promise.all([
  faceapi.nets.tinyFaceDetector.loadFromUri('/models'),
  faceapi.nets.faceLandmark68TinyNet.loadFromUri('/models')
]).then(startVideo);

function startVideo() {
  navigator.mediaDevices.getUserMedia({ video: {} })
    .then((stream) => {
      video.srcObject = stream;
    })
    .catch(err => console.error("Webcam error: ", err));
}

video.addEventListener('play', () => {
  canvas.width = video.videoWidth;
  canvas.height = video.videoHeight;

  const glassesImg = new Image();
  glassesImg.src = 'assets/glasses.png';

  setInterval(async () => {
    const detections = await faceapi.detectSingleFace(video, new faceapi.TinyFaceDetectorOptions())
      .withFaceLandmarks(true);

    context.clearRect(0, 0, canvas.width, canvas.height);

    if (detections) {
      const landmarks = detections.landmarks;
      const leftEye = landmarks.getLeftEye();
      const rightEye = landmarks.getRightEye();

      const eyeWidth = Math.abs(rightEye[3].x - leftEye[0].x) * 1.6;
      const eyeHeight = eyeWidth * 0.4;
      const centerX = (leftEye[0].x + rightEye[3].x) / 2 - eyeWidth / 2;
      const centerY = (leftEye[0].y + rightEye[3].y) / 2 - eyeHeight / 2;

      context.drawImage(glassesImg, centerX, centerY, eyeWidth, eyeHeight);
    }
  }, 100);
});
