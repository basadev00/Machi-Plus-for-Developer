![](Documentation~/resources/MachiPlus-logos_Developer.png)


# Machi Plus For Developer
Machi Plus For Developer は、[PLATEAU](https://www.mlit.go.jp/plateau/)のLOD1建物データのテクスチャをMachiPlusプラットフォームから取得する開発者用ツールです。  
Machi PlusプラットフォームにためられているLOD1のテクスチャを取得・適用することができます。  
Machi Plus For Developer を利用することで、PLATEAUのLOD1の建物を実世界に近づけた景観にすることにより、開発するアプリケーションによるユーザーの体験価値を高めることができます。  
**Machi Plus ForDeveloper は [PLATEAU SDK for Unity](https://github.com/Project-PLATEAU/PLATEAU-SDK-for-Unity) を前提とするアドオンです。** 

![](Documentation~/resources/MachiPlusの流れ.png)

# MachiPlusで現在対応しているエリア
 - 愛知県名古屋市大須周辺エリア (メッシュコード 52365781,52365791 の一部)  
<p align="left">
<img width="493" alt="名古屋大須周辺エリア" src="/Documentation~/resources/名古屋大須周辺エリア.png">
</p>

# サンプルシーン
MachiPlusForDeveloper/Samplesフォルダ配下のシーンを使ってツールの動作を確認が可能です。

# セットアップ環境

## 検証済環境
### 推奨OS環境
- Windows 11

### Unity バージョン
- Unity 2021.3.27f1
    - Unity 2021.3系であれば問題なく動作する見込みです。

### レンダリングパイプライン
- Built-in Rendering Pipeline

## PLATEAU SDKバージョン
- [PLATEAU SDK for Unity v2.0.2-alpha](https://github.com/Project-PLATEAU/PLATEAU-SDK-for-Unity/releases/tag/v2.0.2-alpha)
    - これ以外のバージョンを使用する場合、機能が利用できない可能性があります。

## 導入手順

### 1. Unityでのプロジェクト作成

Unity バージョン 2021.3.27f1で「3D」のテンプレートから Unity プロジェクトを作成してください。  



### 2. PLATEAU SDK for Unityのインストール

PLATEAU SDK-Toolkits for Unityを利用するにあたり、事前にPLATEAU SDKのインストールが必要となります。  
TarballかGitHub URLからインストールをする必要があります。詳細はPLATEAU SDKのドキュメンテーションをご参照ください。  
[PLATEAU SDK for Unity利用マニュアル](https://project-plateau.github.io/PLATEAU-SDK-for-Unity/)

PLATEAU SDKを利用し、対応エリアの3D都市モデルをシーン上へ配置してください。（ダウンロードの際に属性情報を含めるようにしてください）

### 3. Machi Plus For Developer のインストール

1. リリースされているファイル[Machi-Plus-For-Developer-v1.0.0.unitypackage](https://github.com/basadev00/Dev_MachiPlusForDeveloper/releases/download/v1.0.0/Machi-Plus-For-Developer-v1.0.0.unitypackage)
をダウンロードします。
2. Unityエディタを開き1でダウンロードしたUnityPackageインポートします。（以下のどちらかの方法でインポートしてください）
   - Unityエディタを開いた状態で、UnityPackageをダブルクリック
   - UnityエディタのProject上で右クリックをし、Import Package -> Custom PackageでUnitypackageファイルを選択。　　

　インポート時は全てのファイルにチェックを入れてImportを押下します。

## Machi Plus For Developer の使い方

1. テクスチャを反映させたい3D都市モデルのLOD1の建物を選択します。（複数選択可）
2. Unityのメニューバーから PLATEAU → ExecuteUserGeneratedTextureLoader を選択します。
3. コンソールウィンドウに「実行が終了しました。」と表示されるまで待ちます。（選択した建物のうち、適用できる建物のみにテクスチャが反映されます）

## ライセンス
- 本リポジトリはMITライセンスで提供されています。

## 注意事項/利用規約
- 本リポジトリの利用により生じた損失及び損害等について、提供者はいかなる責任も負わないものとします。
