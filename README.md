# 🎬 Film & Dizi Takip Sistemi

.NET MAUI teknolojisi kullanılarak geliştirilmiş, kullanıcı ve yönetici (Admin) modüllerine sahip bir **film/dizi takip uygulaması**.

Bu proje; kullanıcıların izledikleri film ve dizileri takip edebilmesini, arama/filtreleme yapabilmesini, yorum ve puan verebilmesini; yöneticilerin ise film, dizi ve kullanıcı yönetimini gerçekleştirebilmesini sağlar. :contentReference[oaicite:0]{index=0} :contentReference[oaicite:1]{index=1}

---

## GitHub Description (Kısa Açıklama)

> .NET MAUI tabanlı Film & Dizi Takip Sistemi — Kullanıcı Girişi, İzleme Geçmişi, Bölüm Takibi, Arama/Filtreleme, Yorum/Puanlama, Admin Paneli, REST API + SQL Server.

> Alternatif (EN):  
> .NET MAUI movie & TV series tracking app with user/admin modules, watch history, episode tracking, search/filtering, ratings/comments, REST API and SQL Server integration.

---

## Proje Amacı

Bu proje, kullanıcıların izledikleri film ve dizileri organize bir şekilde takip etmelerine olanak tanımak, içerik yönetimi yapmalarını sağlamak ve kullanıcı deneyimini artıracak ek işlevlerle zenginleştirilmiş bir arayüz sunmak amacıyla geliştirilmiştir. Uygulama .NET MAUI ile geliştirilmiş olup API destekli ve modüler bir yapıdadır. :contentReference[oaicite:2]{index=2} :contentReference[oaicite:3]{index=3}

---

## Öne Çıkan Özellikler

### 👤 Kullanıcı Modülü
- Kullanıcı girişi ve kayıt sistemi
- İzlenen içerikler listesi (izleme geçmişi)
- Dizi bölüm/sezon takibi (kaldığı yerden devam etme)
- Yeni bölüm / sezon bildirim sistemi
- Arama ve filtreleme (kategori / isim bazlı)
- Yorum yapma ve 1–5 arası puanlama
- Film / dizi detay sayfaları
- Benzer içerik önerileri (özellikle film/dizi detaylarında) :contentReference[oaicite:4]{index=4} :contentReference[oaicite:5]{index=5}

### 🛠️ Yönetici (Admin) Modülü
- Film ekleme / listeleme / filtreleme / güncelleme / silme
- Dizi ekleme / güncelleme / silme
- Kullanıcı listesi görüntüleme
- Kullanıcı silme
- Kullanıcılara ait içerik, yorum ve puanları görüntüleme
- Yönetici paneli üzerinden CRUD tabanlı içerik yönetimi :contentReference[oaicite:6]{index=6} :contentReference[oaicite:7]{index=7}

---

## Kullanılan Teknolojiler

### Backend / API
- ASP.NET Core Web API (RESTful API)
- HttpClient ile istemci–sunucu iletişimi
- SQL Server (kalıcı veri saklama) :contentReference[oaicite:8]{index=8} :contentReference[oaicite:9]{index=9}

### Frontend / UI
- .NET MAUI (tek kod tabanı; Windows / Android / iOS desteği)
- XAML tabanlı arayüz tasarımı
- CollectionView tabanlı listeleme yapıları :contentReference[oaicite:10]{index=10} :contentReference[oaicite:11]{index=11} :contentReference[oaicite:12]{index=12}

### Mimari / Yapısal Yaklaşım
- API destekli istemci uygulaması
- Model sınıflarının doğrudan kullanımı (Film, Kategori, Kullanici vb.)
- Modüler sayfa yapısı (Giriş, Arama, İzleme Geçmişi, Detay sayfaları, Admin sayfaları) :contentReference[oaicite:13]{index=13} :contentReference[oaicite:14]{index=14}

---

## Mimari Yapı (Özet)

Proje, .NET MAUI istemcisi ile ASP.NET Core Web API arasında çalışan bir yapıya sahiptir:

- **MAUI UI Katmanı:** Kullanıcı arayüzü, sayfalar, form işlemleri, listeleme ekranları
- **API Katmanı:** Film, dizi, kategori, kullanıcı ve ilgili işlemler için REST endpoint’leri
- **Veri Katmanı:** SQL Server üzerinde film, kategori, kullanıcı vb. tablolar
- **İletişim Katmanı:** `HttpClient` ile API çağrıları (GET/POST/PUT/DELETE) :contentReference[oaicite:15]{index=15} :contentReference[oaicite:16]{index=16}

---

## Veri Modeli ve Ek Özellikler (Özet)

### Temel Modeller
- `Film`
- `Kategori`
- `Kullanici`
- (Arama sonuçları için) `Arama` modeli / benzeri yapı :contentReference[oaicite:17]{index=17} :contentReference[oaicite:18]{index=18}

### Ek Özellikler
- `ImageUrl` ile film görsellerini listeleme
- `FilmUrl` ile video bağlantısı desteği
- `KategoriId` yerine `KategoriAd` gösterimi (listeleme tarafında iyileştirme)
- Yönetici paneli üzerinden tüm CRUD işlemleri :contentReference[oaicite:19]{index=19}

---

## Uygulama Akışı (Kısa Teknik Özet)

- **Giriş / Kayıt:** Kullanıcı hesabı ile oturum açma veya kayıt olma
- **Ana İçerik Listeleme:** Film ve dizileri listeleme, filtreleme
- **Arama Sayfası:** Anahtar kelime ile film/dizi arama, sonuçtan detay sayfasına yönlendirme
- **Detay Sayfası:** İçerik bilgileri, yorum/puan ve ilgili içerikler
- **İzleme Geçmişi:** Film/dizi geçmişini görüntüleme, tekli/toplu silme, dizilerde kaldığı yerden devam
- **Admin Paneli:** Film, dizi ve kullanıcı yönetimi işlemleri (CRUD) :contentReference[oaicite:20]{index=20} :contentReference[oaicite:21]{index=21} :contentReference[oaicite:22]{index=22}

---

## Çalışma Durumu / Temel İşlevler

Proje raporuna göre uygulama çalışmaktadır; kullanıcı girişi, veri listeleme, ekleme, güncelleme, silme işlemleri ve API istemci bağlantısı sorunsuz şekilde sağlanmıştır. Ayrıca temel gereksinimler (kayıt/giriş, arama, yorum, filtreleme, geçmiş, bölüm takibi, admin yönetimi vb.) karşılanmaktadır. :contentReference[oaicite:23]{index=23} :contentReference[oaicite:24]{index=24}

---

## Uygulama Ekranları (README için Önerilen Başlıklar)

> Bu bölüme kendi ekran görüntülerini ekleyebilirsin.

### 1) Giriş Sayfası
Kullanıcıların sisteme giriş yaptığı ekran. :contentReference[oaicite:25]{index=25}
<img width="945" height="386" alt="image" src="https://github.com/user-attachments/assets/31d28985-88e8-40f3-bcd3-e805b6e1cc3e" />


### 2) Kayıt Ol Sayfası
Yeni kullanıcı oluşturma işlemlerinin yapıldığı ekran. :contentReference[oaicite:26]{index=26}
<img width="945" height="247" alt="image" src="https://github.com/user-attachments/assets/78394fdf-eef1-4657-a850-ce420d135a6b" />


### 3) Ana Sayfa / İçerik Listeleme
Film ve dizilerin listelendiği, filtreleme yapılan ana ekran. :contentReference[oaicite:27]{index=27}
<img width="945" height="357" alt="image" src="https://github.com/user-attachments/assets/45be6a61-e67c-444a-83d2-22893ac0247d" />
<img width="945" height="363" alt="image" src="https://github.com/user-attachments/assets/072ed43f-2680-47ed-8b97-364bcdf14e9e" />



### 4) Arama Sayfası
Kullanıcının anahtar kelime ile arama yapıp sonuçlardan detay sayfasına geçtiği ekran. :contentReference[oaicite:28]{index=28}
<img width="945" height="288" alt="image" src="https://github.com/user-attachments/assets/5c404a72-08f6-4c42-b9a3-62aed8b74c66" />


### 5) Kategori Detay Sayfası
Seçilen kategoriye ait film/dizi içeriklerinin listelendiği ekran. :contentReference[oaicite:29]{index=29}
<img width="945" height="474" alt="image" src="https://github.com/user-attachments/assets/3ae19c40-6a9d-49a0-beaa-b0dbe4b9956c" />


### 6) Film Detay Sayfası
Film bilgileri, yorum/puan ve varsa ilgili içerik önerileri. :contentReference[oaicite:30]{index=30} :contentReference[oaicite:31]{index=31}
<img width="945" height="495" alt="image" src="https://github.com/user-attachments/assets/62ce30c1-454f-456f-a937-18d51f39b131" />
<img width="945" height="418" alt="image" src="https://github.com/user-attachments/assets/08855428-47b3-4b46-a5f0-e96f3d5ba6d0" />



### 7) Dizi Detay Sayfası
Dizi bilgileri, bölüm/sezon ilerleme ve devam etme işlemleri. :contentReference[oaicite:32]{index=32}
<img width="945" height="432" alt="image" src="https://github.com/user-attachments/assets/7ff1ac5d-3e9f-4d7b-a729-a04e536e6007" />
<img width="945" height="430" alt="image" src="https://github.com/user-attachments/assets/9307489c-ae53-4f5b-8866-48020d32e5da" />

<img width="945" height="499" alt="image" src="https://github.com/user-attachments/assets/29390753-8c82-4e54-ad51-78d0e2e5f8ad" />
<img width="945" height="416" alt="image" src="https://github.com/user-attachments/assets/0ffabbab-f213-4343-aa36-6cce7f1d8d3e" />





### 8) İzleme Geçmişi Sayfası
Geçmişte izlenen içeriklerin listelenmesi ve yönetimi (tekli/toplu silme, devam etme). :contentReference[oaicite:33]{index=33}
<img width="945" height="483" alt="image" src="https://github.com/user-attachments/assets/a63c007d-f187-455c-b13f-1a7e64596f24" />


### 9) Admin Paneli - Film Takibi
Film ekleme, listeleme, güncelleme, silme ve filtreleme işlemleri. :contentReference[oaicite:34]{index=34}
<img width="945" height="469" alt="image" src="https://github.com/user-attachments/assets/010c32b3-734f-4c53-92ce-9718dda8e281" />

<img width="945" height="442" alt="image" src="https://github.com/user-attachments/assets/a2d6ef62-a22f-455a-addd-fe7f8e32ffec" />
<img width="945" height="445" alt="image" src="https://github.com/user-attachments/assets/b696d361-02ce-453a-ad5b-b8c766df6f07" />




### 10) Admin Paneli - Dizi Takibi
Dizi yönetimi işlemleri (ekle / güncelle / sil). :contentReference[oaicite:35]{index=35}
<img width="945" height="491" alt="image" src="https://github.com/user-attachments/assets/8f78b6ca-d0fa-4896-9233-af811028d330" />
<img width="945" height="469" alt="image" src="https://github.com/user-attachments/assets/489a94f4-da91-43da-91ea-47ac5c8f9448" />



### 11) Admin Paneli - Kullanıcı Takibi
Kullanıcı listesi, kullanıcı silme ve kullanıcı içerik/yorum/puan görüntüleme. :contentReference[oaicite:36]{index=36}
<img width="945" height="474" alt="image" src="https://github.com/user-attachments/assets/1bd4e0d6-2dab-41a3-803c-04cbbccc29e9" />


---

## Kurulum ve Çalıştırma (Genel)

### 1) Gereksinimler
- Visual Studio 2022 (MAUI workload kurulu)
- .NET SDK (proje sürümüne uygun)
- SQL Server
- ASP.NET Core Web API projesi (backend)

### 2) Veritabanını Hazırla
- SQL Server üzerinde veritabanını oluştur
- Film / Kategori / Kullanici vb. tabloları yapılandır
- API tarafındaki bağlantı ayarlarını düzenle

### 3) Backend API’yi Çalıştır
- Web API projesini aç
- Gerekli paketleri yükle / restore et
- `appsettings.json` bağlantı ayarlarını güncelle
- API’yi başlat

### 4) MAUI Uygulamasını Çalıştır
- MAUI projesini aç
- API base URL adresini güncelle
- Hedef platformu seç (Windows / Android vb.)
- Uygulamayı çalıştır

---

## Proje Klasör Yapısı (Örnek)

```text
FilmDiziTakipSistemi/
├─ Client/                         # .NET MAUI uygulaması
│  ├─ Pages/
│  │  ├─ GirisPage.xaml
│  │  ├─ FilmDiziAramaPage.xaml
│  │  ├─ IzlemeGecmisiPage.xaml
│  │  ├─ FilmDetayPage.xaml## Uygulama Ekranları (README için Önerilen Başlıklar)

> Bu bölüme kendi ekran görüntülerini ekleyebilirsin.

### 1) Giriş Sayfası
Kullanıcıların sisteme giriş yaptığı ekran.

<p align="center">
  <img src="https://github.com/user-attachments/assets/31d28985-88e8-40f3-bcd3-e805b6e1cc3e" width="900" alt="Giriş Sayfası" />
</p>

---

### 2) Kayıt Ol Sayfası
Yeni kullanıcı oluşturma işlemlerinin yapıldığı ekran.

<p align="center">
  <img src="https://github.com/user-attachments/assets/78394fdf-eef1-4657-a850-ce420d135a6b" width="900" alt="Kayıt Ol Sayfası" />
</p>

---

### 3) Ana Sayfa / İçerik Listeleme
Film ve dizilerin listelendiği, filtreleme yapılan ana ekran.

<p align="center">
  <img src="https://github.com/user-attachments/assets/45be6a61-e67c-444a-83d2-22893ac0247d" width="48%" alt="Ana Sayfa - Listeleme 1" />
  <img src="https://github.com/user-attachments/assets/072ed43f-2680-47ed-8b97-364bcdf14e9e" width="48%" alt="Ana Sayfa - Listeleme 2" />
</p>

---

### 4) Arama Sayfası
Kullanıcının anahtar kelime ile arama yapıp sonuçlardan detay sayfasına geçtiği ekran.

<p align="center">
  <img src="https://github.com/user-attachments/assets/5c404a72-08f6-4c42-b9a3-62aed8b74c66" width="900" alt="Arama Sayfası" />
</p>

---

### 5) Kategori Detay Sayfası
Seçilen kategoriye ait film/dizi içeriklerinin listelendiği ekran.

<p align="center">
  <img src="https://github.com/user-attachments/assets/3ae19c40-6a9d-49a0-beaa-b0dbe4b9956c" width="900" alt="Kategori Detay Sayfası" />
</p>

---

### 6) Film Detay Sayfası
Film bilgileri, yorum/puan ve varsa ilgili içerik önerileri.

<p align="center">
  <img src="https://github.com/user-attachments/assets/62ce30c1-454f-456f-a937-18d51f39b131" width="48%" alt="Film Detay Sayfası 1" />
  <img src="https://github.com/user-attachments/assets/08855428-47b3-4b46-a5f0-e96f3d5ba6d0" width="48%" alt="Film Detay Sayfası 2" />
</p>

---

### 7) Dizi Detay Sayfası
Dizi bilgileri, bölüm/sezon ilerleme ve devam etme işlemleri.

<p align="center">
  <img src="https://github.com/user-attachments/assets/7ff1ac5d-3e9f-4d7b-a729-a04e536e6007" width="48%" alt="Dizi Detay Sayfası 1" />
  <img src="https://github.com/user-attachments/assets/9307489c-ae53-4f5b-8866-48020d32e5da" width="48%" alt="Dizi Detay Sayfası 2" />
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/29390753-8c82-4e54-ad51-78d0e2e5f8ad" width="48%" alt="Dizi Detay Sayfası 3" />
  <img src="https://github.com/user-attachments/assets/0ffabbab-f213-4343-aa36-6cce7f1d8d3e" width="48%" alt="Dizi Detay Sayfası 4" />
</p>

---

### 8) İzleme Geçmişi Sayfası
Geçmişte izlenen içeriklerin listelenmesi ve yönetimi (tekli/toplu silme, devam etme).

<p align="center">
  <img src="https://github.com/user-attachments/assets/a63c007d-f187-455c-b13f-1a7e64596f24" width="900" alt="İzleme Geçmişi Sayfası" />
</p>

---

### 9) Admin Paneli - Film Takibi
Film ekleme, listeleme, güncelleme, silme ve filtreleme işlemleri.

<p align="center">
  <img src="https://github.com/user-attachments/assets/010c32b3-734f-4c53-92ce-9718dda8e281" width="48%" alt="Admin Film Takibi 1" />
  <img src="https://github.com/user-attachments/assets/a2d6ef62-a22f-455a-addd-fe7f8e32ffec" width="48%" alt="Admin Film Takibi 2" />
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/b696d361-02ce-453a-ad5b-b8c766df6f07" width="900" alt="Admin Film Takibi 3" />
</p>

---

### 10) Admin Paneli - Dizi Takibi
Dizi yönetimi işlemleri (ekle / güncelle / sil).

<p align="center">
  <img src="https://github.com/user-attachments/assets/8f78b6ca-d0fa-4896-9233-af811028d330" width="48%" alt="Admin Dizi Takibi 1" />
  <img src="https://github.com/user-attachments/assets/489a94f4-da91-43da-91ea-47ac5c8f9448" width="48%" alt="Admin Dizi Takibi 2" />
</p>

---

### 11) Admin Paneli - Kullanıcı Takibi
Kullanıcı listesi, kullanıcı silme ve kullanıcı içerik/yorum/puan görüntüleme.

<p align="center">
  <img src="https://github.com/user-attachments/assets/1bd4e0d6-2dab-41a3-803c-04cbbccc29e9" width="900" alt="Admin Kullanıcı Takibi" />
</p>

---

## Proje Klasör Yapısı (Örnek)

```text
FilmDiziTakipSistemi/
├─ Client/                         # .NET MAUI uygulaması
│  ├─ Pages/
│  │  ├─ GirisPage.xaml
│  │  ├─ FilmDiziAramaPage.xaml
│  │  ├─ IzlemeGecmisiPage.xaml
│  │  ├─ FilmDetayPage.xaml
│  │  ├─ DiziDetayPage.xaml
│  │  └─ KategoriDetayPage.xaml
│  ├─ Models/
│  ├─ Services/
│  └─ App.xaml
├─ Api/                            # ASP.NET Core Web API
│  ├─ Controllers/ (veya Endpoints)
│  ├─ Models/
│  ├─ Data/
│  └─ Program.cs
├─ Database/
│  └─ SQL Scripts/
└─ README.md
│  │  ├─ DiziDetayPage.xaml
│  │  └─ KategoriDetayPage.xaml
│  ├─ Models/
│  ├─ Services/
│  └─ App.xaml
├─ Api/                            # ASP.NET Core Web API
│  ├─ Controllers/ (veya Endpoints)
│  ├─ Models/
│  ├─ Data/
│  └─ Program.cs
├─ Database/
│  └─ SQL Scripts/
└─ README.md
