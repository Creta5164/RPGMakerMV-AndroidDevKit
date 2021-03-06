# RPGMakerMV-AndroidDevKit
이것은 RPG Maker MV를 안드로이드로 빌드하는 비공식 개발킷입니다.  
2019년 02월 09일 작성됨. (v1.0)  
2019년 05월 06일 업데이트 됨. (v1.1)  
2019년 11월 23일 업데이트 됨. (v1.1b)  
2019년 12월 14일 업데이트 됨. (v1.2)  
차후에는 업데이트로 인해 동작이 원활하지 않을 수 있습니다.

# v1.0을 사용하셨던 분들에게

v1.1에는 게임의 세이브를 실제 안드로이드 로컬 스토리지에 저장하는 기능이 생겼습니다.  
v1.0 에서 v1.1 로 마이그레이션 하는 가이드는 **`v1.0 에서 v1.1 로 마이그레이션 하는 법`** 섹션에 있습니다.

# 준비물

- 안드로이드 스튜디오
- RPG MV 프로젝트 폴더

# 준비

이 저장소를 복제하세요. (아니면 ZIP 파일로 받아 압축을 푸세요.)

## 만약 Windows를 사용하지 않는다면...

다른 플랫폼에서는 `ApplicationPackager` 를 빌드해야 합니다, [ApplicationPackager 빌드하기](#ApplicationPackager-빌드하기)를 확인하세요.

## RPG MV

RPG MV 프로젝트를 이 폴더에 넣으세요.  
그러고나서 프로젝트 폴더를 `MV`로 변경하세요.  

그리고 해당 프로젝트에는 `MVRequrements`의 `android-loader.js` 플러그인을 추가해야 합니다.  
이 플러그인을 플러그인 목록 중에 제일 위에 두세요.

## 안드로이드 스튜디오

새 프로젝트를 만들때 `Empty activity` 로 만들어야 합니다.

![](img/package-name.png)  
패키지 이름을 기억하세요.

액티비티 이름을 `MainView`, 레이아웃의 이름을 `activity_main_view`로 바꿔서 프로젝트를 생성하면 됩니다.

안드로이드 스튜디오가 프로젝트 생성하는 작업이 끝나면 (Gradle 작업까지 포함), 프로젝트 탐색 뷰를 바꿔야 합니다.  
왼쪽의 Project 탭을 클릭하고, 윗쪽의 드롭다운을 `Android`로 바꿔줍니다.  
![](img/change-view.png)

이제 `AndroidRequirements/AndroidManifest.xml` 파일의 내용을 이 프로젝트의 매니페스트에 덮어써줍니다.
`YOUR_PACKAGE_NAME_HERE`라고 되어있는 부분을 꼭 패키지 이름으로 바꾸세요.  
![](img/manifest.png)

`AndroidRequirements/styles.xml`의 내용을 복사해서 프로젝트의 `styles.xml` 내용을 덮어써줍니다.  
![](img/styles.png)  
또 `AndroidRequirements/activity_main_view.xml`의 내용을 복사해서 프로젝트의 `activity_main_view.xml` 내용을 덮어써줍니다.  
![](img/layout.png)

이제 매니페스트의 내용에 빨간 텍스트가 표시되지 않는다면 성공입니다.

이제 `MainView`를 열어봅시다.  
![](img/mainviewloc.png)

이것도 똑같이 `AndroidRequirements/MainView.java` 파일의 내용을 여기에 덮어씌워줍니다.  
역시 `YOUR_PACKAGE_NAME_HERE`를 패키지 이름으로 바꿔주세요.  
![](img/mainview.png)

그런 다음, 폴더를 우클릭 해서 New - Java Class 메뉴를 통해 새 클래스를 만들어줍니다.  
이름은 `SaveDataManager`으로 해주세요.  
![](img/create-new-class.png)  
![](img/create-new-class-2.png)

`MainView`에 했던 작업과 동일합니다, `AndroidRequirements/SaveDataManager.java`에 있는 내용을 만든 파일 안에 붙여넣으세요.  
역시 `YOUR_PACKAGE_NAME_HERE`를 패키지 이름으로 바꿔야 합니다.  

거의 다 끝났습니다, `assets` 폴더를 만들어봅시다.  
(res 폴더 우클릭, `New - directory - Assets directory`)  
창이 나타난다면, `Finish` 버튼을 클릭하세요.  
![](img/create-assets-directory.png)

## 마무리하기

### 안드로이드 패키징 자동화 환경 설정하기

1.2 버전 업데이트 이후로 MV 프로젝트를 안드로이드 프로젝트에 자동으로 적용해줄 수 있는 기능이 추가됐습니다.  
MV에서 프로젝트를 저장하고, 안드로이드 스튜디오에서 바로 테스트 버튼을 누르면, 수정했던 부분이 바로 적용된 채로 테스트를 바로 할 수 있습니다!  
이제 이 환경을 준비해봅시다.

초록색 망치 아이콘의 우측에 있는 드롭다운 메뉴를 열고, `Edit Configurations...`를 선택합니다.
![](img/automation1.png)

그러면 `Run/Debug Configurations` 창이 나타납니다.  
![](img/automation2.png)

`Before launch: Gradle-aware Make` 탭 아래에 있는 `+` 아이콘을 클릭하고 `Run External Tool`를 선택하세요.  
![](img/automation3.png)

그러면 `External Tools` 창이 나타납니다.  
이 창에서 보이는 `+` 아이콘을 클릭하세요.  
![](img/automation4.png)

그러면 `Create Tool` 창이 나타납니다. (또)  
`Name` 항목에 `ApplicationPackager`라고 기입해주세요.
![](img/automation5.png)

이제 `Tool Settings` 부분을 설정해야 됩니다.  

---

### - `Program` 항목
폴더 아이콘을 클릭하고, 현재 작업하고 있는 폴더 안의 `<작업 폴더>/ApplicationPackager/bin/Release/netcoreapp2.2/win-x64/publish/ApplicationPackager.exe` 를 선택하세요. (아래처럼요)  
![](img/automation6.png)  

### - `Arguments` 항목
`-f`만 써주세요.  

### - `Working directory` 항목
폴더 아이콘을 클릭하고, 현재 작업하고 있는 폴더를 선택해주세요.

### - `Advenced Options` 탭
`Synchornize files after execution`과 `Open console for tool output` 체크박스를 체크해줍니다.

---

이 과정을 모두 마쳤다면, 아래와 같은 모습이 됩니다.  
![](img/automation7.png)  

`Create Tool`창과 `External Tools`창의 `OK`버튼을 클릭해서 두 창을 모두 닫습니다.  

그런 다음, 마지막으로 추가했던 `External Tool` 항목을 제일 위로 오게 배치하세요.  
(항목을 드래그 하거나 화살표 아이콘을 클릭해서 순서를 변경할 수 있습니다.)  
![](img/automation8.png)

## 테스트하기

와! 드디어 작업이 끝났습니다!  
안드로이드 기기와 USB를 이용해 컴퓨터에 연결한 후
(개발자 모드가 활성화 된 상태로), 초록색 재생 버튼을 클릭하세요.  
![](img/test.png)  
그러고나서 연결한 기기를 선택하고 MV 게임을 확인해보세요.  
> 메모 : 만약에 `ApplicationPackager` 빌드 오류에서 `Please edit 'packager-config.json' to setup packaging environment.`와 같은 문제로 진행을 하지 못한다면, 작업 폴더에 만들어진 `packager-config.json` 설정 파일을 직접 수정해서 빌드 앱이 폴더들을 찾을 수 있도록 설정해주세요.
![](img/automation9.png)  
> - `rpgmv-path` : RPG MV 프로젝트가 있는 폴더를 가리켜야 합니다.  
> - `assets-path` : 안드로이드 프로젝트 안의 `assets` 폴더를 가리켜야 합니다, 보통 `<안드로이드 프로젝트 폴더>/app/src/main/` 폴더 안에 위치합니다.
> 
> ![](img/automation-config.png)


*...잠깐, MV 플러그인에서 오류가 났다!*  
*컴퓨터에서 플레이 할 때는 본 적이 없는데...*  
*이 오류를 어떻게 자세히 확인할 수 있죠?*

만약에 안드로이드 기기가 컴퓨터에 연결된 채로 앱을 실행하고 있는 중이라면...  
Chrome을 실행해서 주소창에 `chrome://inspect`를 칩니다.

그러면 해당 페이지에서 잠시 뒤에 연결된 안드로이드 기기를 볼 수 있습니다!  
`Inspect` 버튼을 클릭해서 콘솔 로그를 확인해보세요.  
![](img/inspect.png)
- 알림 : 원격 디버거는 게임 화면을 표시해주지 못합니다 (HTML Canvas). 안드로이드 기기를 보세요!

더 많은 원격 디버깅에 대한 정보는 여기에서 확인하세요.  
https://developers.google.com/web/tools/chrome-devtools/remote-debugging/webviews

# v1.0 에서 v1.1 로 마이그레이션 하는 법

`MVRequirements` 폴더 안의 `android-loader-for-migration.js` 파일을 받고, 기존에 있던 `android-loader.js`를 같은 이름으로 덮어씁니다.  
그런 다음, `Android studio` 섹션의 `MainView` 부분부터 따라가세요.  

# `ApplicationPackager` 빌드하기

# 준비물

- .NET Core SDK 2.0 또는 그 이상

# 빌드하기

터미널(cmd)를 키고, `ApplicationPackager` 폴더로 이동합니다.  
그리고 아래의 명령을 실행하세요.

```
dotnet restore
dotnet publish -c Release --runtime RID
```
- `RID` 부분을 사용하는 플랫폼으로 바꿔주세요.  
  지원하는 플랫폼 목록은 아래의 주소에서 확인하실 수 있습니다.  
  https://docs.microsoft.com/ko-kr/dotnet/core/rid-catalog  
  (예 : Mac OS의 RID는 `osx-x64`)  
  ```
  dotnet publish -c Release --runtime osx-x64
  ```

빌드가 끝났으면, 상위 폴더로 이동합니다.  
그리고 터미널에서 `./ApplicationPackager/bin/Release/<.NET Core 버전>/<RID>/publish/ApplicationPackager`를 실행해보세요.

잘 동작한다면, 이것을 지금 사용중인 IDE(안드로이드 스튜디오, XCode 등)에 맞게 빌드하기 전에 이 프로그램이 실행할 수 있도록 **`마무리하기`** 섹션을 참고해서 자동화 환경을 구성하세요.  
이 프로그램은 사용자의 입력을 무시하고 패키징 작업을 진행하는 강제 모드(실행 인자 `-f`)가 포함되어 있습니다.