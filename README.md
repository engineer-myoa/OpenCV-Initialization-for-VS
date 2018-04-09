OpenCV_Initialization-for-VS
===========================

## 소개 / Introduction
Windows, Visual Studio 개발 환경에서 OpenCV를 사용하기 편리하도록 환경변수 및 템플릿을 생성을 지원해주는 응용프로그램입니다.

## 사용방법 / HOW TO USE
### a) English
1. Get OpenCV windows release https://opencv.org/releases.html
2. Extract archive file
3. Run OpenCV Initializer
4. Browse your opencv root path
5. Select your platform and content to use
6. Click "CV Init" button.
7. If Visual Studio is on, Restart it.
8. Make Project via. Create -> New Project -> C++ -> OpenCV Template
9. Done

### b) 한국어
1. opencv 최상위 경로 지정
2. 사용 platform설정 (x86, x64, vs2017, vs2015)
3. 포함할 컨텐츠 설정(main.cpp, lena.png, wildlife.mp4)
4. 후 CV Init버튼을 누르면 동작합니다.
5. 지시에 따라 Visual Studio를 재시작합니다
6. OpenCV Template이 프로젝트 템플릿 목록에 추가되어있습니다.


## 적용 사항 / Affected things
1. "PATH" which in system environment path added OpenCV bin folder
2. In OpenCV Template, pre-initialized OpenCV Header path and lib path
3. In project folder included some content what is you were checked.

## 기타 사항 / Trouble Shoot

1. If you applied x64 platform, you must changed build architecture(or platform) to x64 when you created project.
2. If you seen SDK Version Not found error with Compile time or Typing time, 
Solution Right Click -> "Target Solution Change" will be fix it.
3. Successfully Works tested in  win7 32bit, win7 64bit, win10 32bit, win10 64bit.
