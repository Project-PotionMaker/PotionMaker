<div align="center">
<h2>PotiomMaker 🧪</h2>

최대 4명의 플레이어들이 협력하여 포션을 제조하고 판매하여, 점차 더 큰 포션 상점을 꾸리고 성장해 나가는 판타지 협동 시뮬레이션 게임, PotionMaker입니다!<br>
해당 프로젝트는 SKKU Com2us SAY 1기에서 진행한 프로젝트입니다.🍀

#### ↓↓↓↓↓ 아래 이미지를 클릭하면 PotionMaker 플레이 영상을 유튜브에서 보실 수 있습니다. ↓↓↓↓↓
[![플레이 영상](https://img.youtube.com/vi/BfHhdrasDP4/maxresdefault.jpg)](https://www.youtube.com/watch?v=BfHhdrasDP4)<br>

</div><br>

## 목차
  - [개요](#개요) 
  - [게임 설명](#게임-설명)
  - [사용 기술](#사용-기술)
  - [게임 플레이](#게임-플레이)
<br>

## 개요
| **프로젝트 명** | PotionMaker |
|:---:|:---:|
| **프로젝트 기간** | 2025.07 - 진행중 |
| **팀원** | 김영석, 박우영, 심형준, 이상진, 최민규 |
| **기술 스택** | <img src="https://img.shields.io/badge/Unity-6000.0.58f2-000000?style=for-the-badge&logo=unity" height="25"> <img src="https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white" height="25"> <img src="https://img.shields.io/badge/Github-181717?style=for-the-badge&logo=github" height="25"> <img src="https://img.shields.io/badge/Github Actions-363636?style=for-the-badge&logo=githubactions" height="25"> <br> <img src="https://img.shields.io/badge/Google Gemini-000000?style=for-the-badge&logo=googlegemini" height="25"> <img src="https://img.shields.io/badge/Claude-000000?style=for-the-badge&logo=claude" height="25"> <img src="https://img.shields.io/badge/Perplexity-000000?style=for-the-badge&logo=perplexity" height="25"> |
| **플랫폼 및 장르** | <img src="https://img.shields.io/badge/Platform-Windows-lightgrey?style=for-the-badge" height="25"> <img src="https://img.shields.io/badge/Genre-3D%20Co--op%20Simulation-red?style=for-the-badge" height="25"> |
<br>

## 게임 설명
|![image](https://github.com/user-attachments/assets/f100ad38-ac98-4700-bba2-926458e942dd)|![image](https://github.com/user-attachments/assets/838cfc82-12ac-408c-8b82-2d6555dcd726)|
|:---:|:---:|
|로비 화면|인게임 화면|

#### **우당탕탕 왁자지껄 포션 상점을 친구들과 함께 경영하세요!** 🐿️<br>

PotionMaker는 최대 4인의 플레이어가 협력하여 재료를 손질하고, 포션을 제조하여 손님에게 판매하는 로컬/온라인 멀티플레이 협동 시뮬레이션 게임입니다. <br><br>

#### 1. 전략적인 영업 준비 🕰️ 📝
- 영업 시작 전, 신문을 통해 오늘의 인기 포션 트렌드를 파악해야 합니다.
- 벌어들인 코인으로 마켓에서 더 효율적인 조리기구(자동화 기기 등)를 구매하고, 동선을 고려하여 가구와 기구를 그리드 위에 전략적으로 배치할 수 있습니다. <br>

#### 2. 긴박한 포션 제조와 판매 🗺️
- 주문이 들어오면 **재료 채집 -> 손질(절구/분쇄) -> 가공(가열/혼합) -> 병입**의 복잡한 공정을 팀원들과 분업하여 빠르게 처리해야 합니다.
- 손님의 인내심이 떨어지기 전까지 포션을 완성하여 픽업 테이블에 제공해야 합니다.
- 레시피는 상점이 성장할수록 더욱 복잡해집니다. <br>

#### 3. 성장과 이사 시스템 ⚖️
- 모은 재화로 더 넓고 좋은 입지의 상점으로 이사를 갈 수 있습니다.
- 상점이 커질수록 새로운 포션 레시피가 해금되고, 다뤄야 할 재료와 기구의 종류가 늘어납니다. <br>

<br>

## 사용 기술
### 1. Network syncronization using Mirror
- 기존에 사용하던 Photon 라이브러리의 CCU 비용 문제 및 호스트 의존성 문제를 해결하기 위해 **Mirror 라이브러리**를 이용한 네트워크 동기화를 진행했습니다.
- [SyncVar]와 Hook을 활용하여 플레이어 상태를 동기화했습니다.
- 네트워크 대역폭 최적화를 위해 Network Factory 패턴을 적용한 커스텀 오브젝트 풀링(Object Pooling) 시스템을 구현하여 오브젝트 생성/파괴 비용을 최소화했습니다.
<br>

### 2. Data Structure & Algorithms
- 효율적인 다양한 **자료구조**와 **알고리즘**들을 활용하여 인게임 콘텐츠들을 구현했습니다.
- 제작 중인 포션의 레시피 유효성 검사를 효율적으로 수행하기 위해 **Trie**를 사용했습니다.
- 홀 영역 내 손님 AI의 길막힘 현상 방지를 위해 **BFS** 알고리즘을 활용한 경로 탐색을 설계했습니다.
<br>

### 3. DDD(Domain-Driven Design) Architecture
- 복잡한 시뮬레이션 로직과 UI, 데이터 간의 강한 결합을 방지하고 유지보수성을 높이기 위해 **DDD**를 도입했습니다.
- 재화(Currency), 판매량(Sales), 방세(Rent) 등의 데이터들에 대해 도메인, 매니저, 계층을 분리했습니다.
- 매니저는 Domain을 관리하고, UI는 매니저를 통해 데이터의 변화에 대응하도록 설계하여 **단방향 의존성**을 유지했습니다.
<br>

### 4. Asset Management using Addressable
- 빌드의 에셋 의존성을 없애고, 빌드 파일 경량화를 위해 **Addressable** 연동을 진행했습니다.
- Addressable Remote 빌드를 통해 주요 에셋들을 Unity Cloud의 CCD에 업로드하고, 업로드한 에셋을 동적으로 로드/언로드하는 AssetManager를 구현했습니다.
- 비효율적인 전체 재빌드를 방지하고, 에셋을 필요한 시점에만 로드하여 런타임 메모리를 최적화할 수 있었습니다.
<br>

### 5. Generative AI Pipeline
- 소규모 팀의 리소스 제작 한계를 극복하고 개발 효율을 극대화하기 위해 적극적으로 생성형 AI를 활용했습니다.
- **Meshy AI**와 **Tripo**를 활용해 이미지를 3D 모델 및 텍스처로 변환하고, **Blender**와 **Claude**를 연동하여 애니메이션 리깅 및 익스포트 과정을 자동화했습니다. 
- **ComfyUI의 Image-to-Image 워크플로우**를 설계하여, 제작된 **3D 포션 모델을 UI용 2D 아이콘으로 일괄 변환**해 리소스 제작 시간을 단축했습니다. 
- **Gemini Code Assist**를 활용하여 PR 생성 시 **코드 리뷰를 자동화**하고, 리팩토링 제안을 통해 **코드 품질을 지속적으로 관리**했습니다.
<br>


## 게임 플레이
### 조작법
| 구분 | 동작 | 입력 키 (Input) |
| :---: | :---: | :---: |
| **이동** | 플레이어 이동 | <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd>|
| **기본 상호작용** | 물건 들기 / 내려놓기 | <kbd>P</kbd> |
| **특수 상호작용** | 조리기구 사용 / 가구 회전 | <kbd>O</kbd> |
| **시스템** | 투표 / UI 팝업 닫기 | <kbd>Space</kbd> |
<br>




### 주요 화면
#### 1. 영업 준비 단계
|인게임 화면|오늘의 포션 정보|마켓|포션 레시피 사전|
|:---:|:---:|:---:|:---:|
|![image](https://github.com/user-attachments/assets/4e044198-6428-40c6-8c4d-e304aaa4fe58)|![image](https://github.com/user-attachments/assets/e8a567c4-d51a-4d89-8f51-e4e4091af49a)|![image](https://github.com/user-attachments/assets/9145826a-ef54-4718-97fd-3d9bbf0da469)|![image](https://github.com/user-attachments/assets/f7adcb42-2943-4d98-8af5-586c144b5375)|
|팀 전략에 맞춰 조리기구 및 가구 배치를 변경합니다.|오늘 영업 단계에서 손님들이 주문할 포션 리스트를 조회합니다.|조리기구와 가구 구매 및 이사가 가능합니다.|해금된 포션의 레시피를 조회할 수 있습니다.|
<br>

#### 2. 영업 단계
![PotionMaker-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/c6d13ed4-c299-42c4-a59f-87bff4a54a6d)
<br><br>

#### 3. 영업 종료 단계
|당일 영업 정산 팝업|영업 성공 팝업|영업 실패 팝업|
|:---:|:---:|:---:|
|![image](https://github.com/user-attachments/assets/cf67d30c-5cf0-472e-a901-543c44d19725)|![image](https://github.com/user-attachments/assets/eebbae88-625e-4ac1-a651-3f8d284ce1e6)|![image](https://github.com/user-attachments/assets/fe802f2c-0426-4089-b239-6a317690b1be)|
|판매한 포션 종류 및 개수, 평판 변화 및 현재 자산을 조회할 수 있습니다.|자산 변화 및 방세 납부일 정보를 조회할 수 있습니다.|자산 변화 및 영업 기간 전체 포션 판매량을 조회할 수 있습니다.|

