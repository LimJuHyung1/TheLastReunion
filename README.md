### 🎯 The Last Reunion (Steam 출시작)
- **장르**: 미스터리 추리 게임
- **개발 기간**: 2024.03 ~ 2024.08
- **사용 기술**: Unity, C#, OpenAI API
- **주요 기능**:
  - OpenAI 기반 AI 대화 시스템
  - 다중 엔딩 구조 설계
  - 증거 수집 및 추론 시스템
- **기여 내용**:
  - 기존에는 사용자가 직접 `auth.json` 파일을 작성해야 했으나,
    게임 실행 시 해당 파일이 자동으로 생성되도록 설계
  - OpenAI API Key 및 Organization 정보를 포함한 파일은 
    사용자 디렉토리에 설치되며, 자체 암호화 및 복호화 시스템을 통해 보안성을 강화
  - 이를 통해 사용자 편의성과 보안성을 동시에 확보함
- **OpenAI API - Unity 연동 API 분석**:
  > ### 🔧 주요 파일 및 기능 설명
  > [Unity에서의 ChatGPT 연동 및 내부 실행 과정 (PDF)](Images/Unity_ChatGPT_Integration_Report.pdf)
  > 
  > #### ✅ `OpenAIApi.cs` – API 통신 관리
  > - OpenAI API와의 HTTP 요청을 비동기 처리
  > - `DispatchRequest`: API 요청 전송 및 응답 처리
  > - `CreateChatCompletion`: 사용자 입력 기반의 ChatGPT 응답 생성
  >
  > #### ✅ `Configuration.cs` – 인증 정보 자동화 및 보안 처리
  > - 기존에는 사용자가 직접 `auth.json`을 작성해야 했으나,
  > 게임 실행 시 해당 파일이 **자동 생성되도록 설계**
  > - `auth.json` 파일은 사용자 디렉토리에 저장되며,
  OpenAI API Key와 Organization 정보를 포함
  > - 민감 정보는 **자체 암호화하여 저장**되며,
  실행 중 복호화 후 사용, 종료 시 메모리에서 제거하여 보안성 강화
  >
  > #### ✅ `DataTypes.cs` – 요청 및 응답 데이터 구조 정의
  > - `CreateChatCompletionRequest`: 대화 요청 데이터 정의
  > - `ChatMessage` 구조체:
  > - `role: user` → 사용자 입력
  > - `role: assistant` → ChatGPT 응답
  > - `role: system` → NPC의 성격 및 대화 스타일을 지정하는 초기 지침

- **성과**: Steam 정식 출시  
- 🛒 [Steam 페이지](https://store.steampowered.com/app/3600510/The_Last_Reunion/)

---

![Slide](./Images/슬라이드1.JPG)
![Slide](./Images/슬라이드2.JPG)
![Slide](./Images/슬라이드3.JPG)
![Slide](./Images/슬라이드4.JPG)
![Slide](./Images/슬라이드5.JPG)
![Slide](./Images/슬라이드6.JPG)
![Slide](./Images/슬라이드7.JPG)
![Slide](./Images/슬라이드8.JPG)
![Slide](./Images/슬라이드9.JPG)
![Slide](./Images/슬라이드10.JPG)
![Slide](./Images/슬라이드11.JPG)
![Slide](./Images/슬라이드12.JPG)
![Slide](./Images/슬라이드13.JPG)
![Slide](./Images/슬라이드14.JPG)
![Slide](./Images/슬라이드15.JPG)
![Slide](./Images/슬라이드16.JPG)
![Slide](./Images/슬라이드17.JPG)
![Slide](./Images/슬라이드18.JPG)
![Slide](./Images/슬라이드19.JPG)
![Slide](./Images/슬라이드20.JPG)
![Slide](./Images/슬라이드21.JPG)
![Slide](./Images/슬라이드22.JPG)
![Slide](./Images/슬라이드23.JPG)
![Slide](./Images/슬라이드24.JPG)
![Slide](./Images/슬라이드25.JPG)
![Slide](./Images/슬라이드26.JPG)
![Slide](./Images/슬라이드27.JPG)
![Slide](./Images/슬라이드28.JPG)
![Slide](./Images/슬라이드29.JPG)
![Slide](./Images/슬라이드30.JPG)
![Slide](./Images/슬라이드31.JPG)
![Slide](./Images/슬라이드32.JPG)
![Slide](./Images/슬라이드33.JPG)
![Slide](./Images/슬라이드34.JPG)
![Slide](./Images/슬라이드35.JPG)
