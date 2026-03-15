# SpotFinder — Proje Tanım ve Mimari Dokümanı

> **Versiyon:** 1.3  
> **Tarih:** Mart 2026  
> **Durum:** Faz 1 — Adım 1 Bekliyor

---

## 1. Proje Vizyonu

SpotFinder, kullanıcıların İstanbul'da gitmek istedikleri mekânları konsept bazlı filtreler aracılığıyla keşfetmelerine olanak tanıyan mobil öncelikli bir mekân keşif ve öneri platformudur. Kullanıcılar şehir ve ilçe seçimi yaparak başlar, ardından bir veya birden fazla etkinlik/konsept filtresi (doğum günü, manzara, romantik akşam yemeği vb.) uygulayarak kişiselleştirilmiş mekân önerileri alır.

Platform günden güne genişleyecek olup mekan sahiplerinin kendi profil ve içeriklerini yönetebileceği bir Venue Owner portalı ilerleyen fazlarda devreye alınacaktır.

---

## 2. Hedef Kitle

- **Birincil kullanıcı:** İstanbul'da sosyal aktivite planlayan bireyler (18–45 yaş)
- **İkincil kullanıcı:** Restoran, kafe, etkinlik mekânı sahipleri (Faz 2)
- **Yönetici:** Platform operatörü (admin paneli üzerinden)

---

## 3. Coğrafi Kapsam

- **Faz 1:** Yalnızca İstanbul (tüm ilçeler)
- **Faz 2+:** Ankara, İzmir ve diğer büyükşehirlere genişleme

---

## 4. Temel Özellikler (Faz 1)

### 4.1 Kullanıcı Sistemi

| Özellik | Detay |
|---|---|
| Kayıt / Giriş | E-posta + şifre |
| Sosyal Giriş | Google OAuth, Apple Sign-In |
| Telefon ile Giriş | OTP (SMS) doğrulama |
| Profil | Ad, fotoğraf, tercihler |
| Dil | Türkçe ve İngilizce (TR / EN) |

### 4.2 Mekân Keşif ve Filtreleme

- **Şehir / İlçe seçimi** ile coğrafi daraltma
- **Konsept filtreleri:** Çoklu seçim destekli (örn. "Doğum günü + Manzara")
- **Filtre tipleri:**
  - **Sistem etiketleri:** Platform tarafından tanımlanmış sabit konseptler (doğum günü, romantik, manzaralı, açık hava, kahvaltı, canlı müzik, vb.)
  - **Custom etiketler:** Admin paneli üzerinden dinamik olarak eklenebilen özel konseptler
- **Sonuç ekranı:** Öneri kartları (fotoğraf, isim, ilçe, konsept etiketleri, puan)
- **Google Maps entegrasyonu:** Mekânı haritada görme, yol tarifi

### 4.3 Mekân Detay Sayfası

- Mekân adı, açıklama, adres, ilçe
- Fotoğraf galerisi
- **Güncel menü fotoğrafları** (ayrı sekme/bölüm)
- **Otopark durumu:** Var / Yok / Vale
- Konsept etiketleri
- Çalışma saatleri
- Google Maps mini harita
- Ortalama puan ve yorum sayısı

### 4.4 Kullanıcı Etkileşimleri

- **Favorilere ekleme:** Mekânı kaydetme
- **Geçmiş ziyaretler:** Kullanıcının daha önce gittiğini işaretleyebildiği liste
- **Mekânı paylaşma:** Deep link ile arkadaşa önerme (WhatsApp, Instagram, kopyala)
- **Yorum sistemi:** Metin bazlı olumlu/olumsuz deneyim paylaşımı + puan (1–5 yıldız)

### 4.5 Push Bildirimler

- Yeni mekân eklendiğinde (ilgi alanına göre)
- Favorilerdeki mekândan kampanya/duyuru
- Sistem duyuruları

### 4.6 Admin Paneli (Web)

- Mekân ekleme, düzenleme, silme
- Menü fotoğrafı yükleme
- Konsept etiketi yönetimi (sistem + custom)
- Kullanıcı yönetimi
- Yorum moderasyonu
- İstatistik dashboard (aktif kullanıcı, en çok aranan konsept, vb.)

---

## 5. Teknik Mimari

### 5.1 Genel Bakış

```
[ Flutter Mobile App ]   [ Admin Web Panel ]
          |                      |
          └──────────┬───────────┘
                     │
             [ AWS API Gateway ]
                     │
     ┌───────────────┼──────────────────┐
     │               │                  │
[ Auth Service ] [ Venue Service ] [ Search Service ] [ Review Service ]
     │               │                  │                   │
     └───────────────┴──────────────────┴───────────────────┘
                     │
         ┌───────────┴───────────┐
  [ PostgreSQL - RDS ]    [ AWS S3 + CloudFront ]
```

### 5.2 Mobil Uygulama

| Bileşen | Teknoloji |
|---|---|
| Framework | Flutter (Dart) |
| Platform | iOS + Android (tek codebase) |
| Harita | Google Maps Flutter SDK |
| Push Bildirim | Firebase Cloud Messaging (FCM) |
| State Management | Bloc / Riverpod |
| HTTP | Dio |
| Yerel Depolama | Hive / SharedPreferences |
| Dil Desteği | flutter_localizations (TR + EN) |

### 5.3 Backend — Mikroservis Mimarisi

Backend, **.NET 9 Web API** tabanlı, **CQRS pattern** ile **MediatR** kütüphanesi kullanılarak geliştirilecektir. Her mikroservis bağımsız deploy edilebilir olup kendi CQRS pipeline'ına sahiptir.

#### 5.3.1 Her Mikroservis İçin Katman Yapısı (ZORUNLU)

Her mikroservis tam olarak 3 projeden oluşur. Aşağıdaki isimlendirme kuralı tüm servisler için geçerlidir:

```
SpotFinder.[ServisAdı].API        → Web API projesi (Controller, Program.cs, appsettings)
SpotFinder.[ServisAdı].Business   → Class Library (Commands, Queries, Handlers, Validators, Models, Services)
SpotFinder.[ServisAdı].Data       → Class Library (Entities, DbContext, Repositories, Migrations)
```

**Katman bağımlılık yönü — kesinlikle bu sıraya uyulacak:**
```
API  →  Business  →  Data
```
- API katmanı yalnızca Business katmanına referans verir
- Business katmanı yalnızca Data katmanına referans verir
- Data katmanı hiçbir katmana referans vermez
- Ters yönde referans kesinlikle yasaktır

**Auth Service klasör örneği:**
```
SpotFinder.AuthService/
├── SpotFinder.AuthService.API/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
├── SpotFinder.AuthService.Business/
│   ├── Commands/        ← RegisterCommand.cs, LoginCommand.cs
│   ├── Queries/         ← GetUserByIdQuery.cs
│   ├── Models/          ← AuthResult.cs, UserDto.cs
│   └── Services/        ← IJwtService.cs, JwtService.cs
├── SpotFinder.AuthService.Data/
│   ├── Context/         ← AppDbContext.cs
│   ├── Entities/        ← User.cs, UserRefreshToken.cs
│   ├── Repositories/    ← IUserRepository.cs, UserRepository.cs
│   └── Migrations/
└── SpotFinder.AuthService.sln
```

#### 5.3.2 Katman İçerikleri ve NuGet Paketleri

**API Katmanı**
- `Controllers/` — HTTP endpoint'leri, yalnızca `_mediator.Send()` çağırır, iş mantığı içermez
- `Program.cs` — DI kayıtları, middleware, JWT konfigürasyonu
- `appsettings.json` — Bağlantı ve JWT ayarları
- NuGet: `Microsoft.AspNetCore.Authentication.JwtBearer`, `Swashbuckle.AspNetCore`

**Business Katmanı**
- `Commands/` — Command + Handler + Validator tek dosyada (örn. `RegisterCommand.cs`)
- `Queries/` — Query + Handler + Validator tek dosyada (örn. `GetUserByIdQuery.cs`)
- `Models/` — Request/Response DTO'ları (örn. `AuthResult.cs`)
- `Services/` — Interface + implementasyon (örn. `IJwtService.cs` + `JwtService.cs`)
- NuGet: `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`, `AutoMapper`, `BCrypt.Net-Next`

**Data Katmanı**
- `Entities/` — EF Core entity sınıfları
- `Context/` — `AppDbContext.cs` (OnModelCreating ile tablo/şema mapping)
- `Repositories/` — `IXRepository.cs` interface + `XRepository.cs` implementasyon
- `Migrations/` — EF Core migration dosyaları (otomatik üretilir)
- NuGet: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`

#### 5.3.3 CQRS Kod Kuralları

Her Command dosyası şu sırayı izler:

```csharp
// 1. Command (record)
public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResult>;

// 2. Handler
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct) { ... }
}

// 3. Validator
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator() { RuleFor(...) }
}
```

Controller sadece şu kalıbı izler — iş mantığı kesinlikle controller'da olmaz:

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

#### 5.3.4 Servisler

**Auth Service** — `SpotFinder.AuthService`
- Kullanıcı kaydı, giriş, token yönetimi
- JWT Access Token (15 dk) + Refresh Token (7 gün)
- Google OAuth 2.0, Apple Sign-In entegrasyonu
- SMS OTP (Twilio veya Netgsm)
- Roller: `user`, `admin` (Faz 2: `venue_owner` rolü eklenir)

**Venue Service** — `SpotFinder.VenueService`
- Mekân CRUD işlemleri
- Menü fotoğrafı yönetimi (AWS S3 presigned URL)
- Otopark durumu, çalışma saatleri
- İlçe ve konsept etiket ilişkilendirmesi

**Search & Filter Service** — `SpotFinder.SearchService`
- Şehir / ilçe bazlı filtreleme
- Konsept etiketi bazlı çoklu filtre
- Sıralama: puan, mesafe, yenilik
- Başlangıçta PostgreSQL full-text search; Faz 2'de Elasticsearch

**Review Service** — `SpotFinder.ReviewService`
- Yorum oluşturma, listeleme
- Puan hesaplama (ortalama güncelleme)
- Moderasyon durumu: `pending` / `approved` / `rejected`

### 5.4 Veritabanı

#### 5.4.1 Genel Bilgiler

| Bileşen | Teknoloji |
|---|---|
| Veritabanı | PostgreSQL 16 |
| Geliştirme / Test | Neon.tech (free tier, cloud PostgreSQL) |
| Production | AWS RDS PostgreSQL (Faz 1 canlıya geçişte) |
| Migration | Entity Framework Core Migrations |
| Şema Stratejisi | Her mikroservis kendi şeması (schema-per-service) |
| ORM | Entity Framework Core 9 |

#### 5.4.2 Geliştirme Veritabanı — Neon.tech Bağlantısı

Faz 1 geliştirme ve test sürecinde aşağıdaki Neon.tech veritabanı kullanılır:

```
Host:     ep-bitter-cloud-a9xk9knc-pooler.gwc.azure.neon.tech
Port:     5432
Database: neondb
Username: neondb_owner
Password: npg_NamhpWnicC82
SSL Mode: Require
```

**EF Core connection string formatı (`appsettings.json`):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=ep-bitter-cloud-a9xk9knc-pooler.gwc.azure.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_NamhpWnicC82;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

**Önemli notlar:**
- Neon.tech free plan test amaçlıdır; sınırlı depolama ve bağlantı kotası vardır
- Her servis kendi şemasını kullanır: `auth`, `venue`, `search`, `review`
- Production'a geçişte yalnızca `appsettings.Production.json` içindeki connection string değişir
- Migration'lar her servis için ayrı ayrı çalıştırılır

**Migration komutları:**
```bash
# Migration oluştur
dotnet ef migrations add InitialCreate \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API

# Neon.tech veritabanına uygula
dotnet ef database update \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API

# Son migration'ı geri al
dotnet ef migrations remove \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API
```

#### 5.4.3 Şema Yapıları

**Auth Şeması (`auth.*`)**
```sql
auth.users
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  email           varchar(255) UNIQUE NOT NULL
  password_hash   varchar(500)
  full_name       varchar(150)
  phone_number    varchar(20)
  avatar_url      varchar(500)
  provider        varchar(50) DEFAULT 'local'   -- local | google | apple
  role            varchar(50) DEFAULT 'user'    -- user | admin
  is_active       boolean DEFAULT true
  created_at      timestamptz DEFAULT now()
  updated_at      timestamptz

auth.user_refresh_tokens
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  user_id         uuid REFERENCES auth.users(id)
  token           varchar(500) NOT NULL
  expires_at      timestamptz NOT NULL
  is_revoked      boolean DEFAULT false
  created_at      timestamptz DEFAULT now()

auth.user_favorites
  user_id         uuid REFERENCES auth.users(id)
  venue_id        uuid NOT NULL
  created_at      timestamptz DEFAULT now()
  PRIMARY KEY (user_id, venue_id)

auth.user_visits
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  user_id         uuid REFERENCES auth.users(id)
  venue_id        uuid NOT NULL
  visited_at      timestamptz DEFAULT now()
```

**Venue Şeması (`venue.*`)**
```sql
venue.districts
  id              serial PRIMARY KEY
  name            varchar(100) NOT NULL
  city            varchar(100) DEFAULT 'İstanbul'

venue.concept_tags
  id              serial PRIMARY KEY
  name_tr         varchar(100) NOT NULL
  name_en         varchar(100) NOT NULL
  is_system       boolean DEFAULT false   -- true: sistem etiketi, false: custom
  is_active       boolean DEFAULT true
  created_at      timestamptz DEFAULT now()

venue.venues
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  name            varchar(200) NOT NULL
  description     text
  district_id     integer REFERENCES venue.districts(id)
  address         varchar(500)
  parking_status  varchar(20)             -- available | unavailable | valet
  lat             decimal(10,8)
  lng             decimal(11,8)
  average_rating  decimal(3,2) DEFAULT 0
  review_count    integer DEFAULT 0
  is_active       boolean DEFAULT true
  created_at      timestamptz DEFAULT now()
  updated_at      timestamptz

venue.venue_photos
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  venue_id        uuid REFERENCES venue.venues(id)
  url             varchar(500) NOT NULL
  is_menu_photo   boolean DEFAULT false
  display_order   integer DEFAULT 0
  created_at      timestamptz DEFAULT now()

venue.venue_concepts
  venue_id        uuid REFERENCES venue.venues(id)
  concept_tag_id  integer REFERENCES venue.concept_tags(id)
  PRIMARY KEY (venue_id, concept_tag_id)
```

**Review Şeması (`review.*`)**
```sql
review.reviews
  id              uuid PRIMARY KEY DEFAULT gen_random_uuid()
  venue_id        uuid NOT NULL
  user_id         uuid NOT NULL
  body            text
  rating          smallint CHECK (rating BETWEEN 1 AND 5)
  status          varchar(20) DEFAULT 'pending'  -- pending | approved | rejected
  created_at      timestamptz DEFAULT now()
  updated_at      timestamptz
```

### 5.5 Cloud & Altyapı (AWS — Production)

| Bileşen | AWS Servisi | Açıklama |
|---|---|---|
| Container Hosting | ECS Fargate | Serverless container, mikroservis başına task |
| API Yönetimi | API Gateway | Rate limiting, auth, routing |
| Veritabanı | RDS PostgreSQL | Managed, otomatik backup |
| Dosya Depolama | S3 | Mekân ve menü fotoğrafları |
| CDN | CloudFront | Görsel dağıtımı (Türkiye edge) |
| Push Bildirim | SNS + FCM | iOS / Android bildirim |
| Container Registry | ECR | Docker image yönetimi |
| Secrets | Secrets Manager | API key, DB şifreleri |
| Logging | CloudWatch | Uygulama ve altyapı logları |

### 5.6 Admin Paneli

- **Teknoloji:** React (Next.js) veya Blazor WebAssembly
- **Hosting:** AWS S3 + CloudFront (static hosting)
- **Auth:** Admin JWT token (ayrı scope)
- **Özellikler:** Mekân yönetimi, etiket yönetimi, yorum moderasyonu, kullanıcı listesi, temel analytics

---

## 6. Güvenlik

- Tüm API iletişimi HTTPS / TLS 1.3
- JWT Access Token (15 dk) + Refresh Token (7 gün)
- Rate limiting: API Gateway üzerinde IP bazlı
- Fotoğraf yükleme: S3 presigned URL (backend bypass)
- Input validation: FluentValidation (backend) + form validation (Flutter)
- SQL Injection koruması: EF Core parametreli sorgular
- Hassas veriler: AWS Secrets Manager (production), appsettings.json (development)

---

## 7. Faz Planlaması

---

### FAZ 1 — MVP Geliştirme Planı (10 Adım)

> Her adım tamamlandığında test edilir ve onaylanır. Bir sonraki adıma geçmeden önce o adımdaki tüm maddeler çalışır durumda olmalıdır.
> AI'a her adım için şu komutu ver: **"Adım X'e geç"** veya **"Adım X'i uygula"**

---

#### ADIM 1 — Proje Altyapısı ve Auth Service Backend

> **Hedef:** Temel proje iskeleti kurulsun, kullanıcı kayıt/giriş API'si çalışır hale gelsin.

**Yapılacaklar:**

- [ ] GitHub repo oluştur (`spotfinder`), monorepo klasör yapısını kur (Bölüm 9)
- [ ] `docs/SpotFinder_Project_Document.md` dosyasını repoya ekle
- [ ] `SpotFinder.AuthService` solution'ını 3 katmanlı olarak oluştur
- [ ] Neon.tech bağlantısını `appsettings.json`'a ekle
- [ ] `auth` şemasındaki tüm tabloları EF Core entity olarak tanımla
- [ ] `InitialCreate` migration'ını oluştur ve Neon.tech'e uygula
- [ ] `UserRepository` ve `IUserRepository` yaz
- [ ] `JwtService` yaz (Access Token + Refresh Token üretimi)

**Endpoint'ler:**

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/v1/auth/register` | E-posta + şifre ile kayıt |
| POST | `/api/v1/auth/login` | E-posta + şifre ile giriş |
| POST | `/api/v1/auth/refresh-token` | Access token yenileme |
| POST | `/api/v1/auth/logout` | Refresh token iptal |
| GET | `/api/v1/auth/me` | Giriş yapmış kullanıcı bilgisi (JWT gerekli) |

**Test kriterleri:**
- Swagger UI açılıyor
- Register endpoint'i Neon.tech `auth.users` tablosuna kayıt ekliyor
- Login endpoint'i geçerli JWT döndürüyor
- `/me` endpoint'i token olmadan 401 dönüyor

---

#### ADIM 2 — Auth Service Sosyal Giriş ve OTP

> **Hedef:** Google, Apple ve SMS OTP giriş yöntemleri çalışır hale gelsin.

**Yapılacaklar:**

- [ ] Google OAuth 2.0 token doğrulama entegrasyonu
- [ ] Apple Sign-In token doğrulama entegrasyonu
- [ ] SMS OTP servisi entegrasyonu (Twilio veya Netgsm)
- [ ] OTP kodu için `auth.otp_codes` tablosu ve migration
- [ ] `provider` alanına göre kayıt/giriş akışı (local / google / apple)

**Endpoint'ler:**

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/v1/auth/google` | Google token ile giriş/kayıt |
| POST | `/api/v1/auth/apple` | Apple token ile giriş/kayıt |
| POST | `/api/v1/auth/otp/send` | SMS OTP gönder |
| POST | `/api/v1/auth/otp/verify` | OTP doğrula ve JWT döndür |

**Test kriterleri:**
- Google token ile giriş yapıldığında `provider = 'google'` kaydı oluşuyor
- OTP SMS gerçekten gönderiliyor (sandbox modu kabul edilir)
- Süresi dolmuş OTP reddediliyor

---

#### ADIM 3 — Venue Service Backend (Temel CRUD)

> **Hedef:** Mekân, ilçe ve konsept etiketi yönetimi API'si çalışır hale gelsin.

**Yapılacaklar:**

- [ ] `SpotFinder.VenueService` solution'ını 3 katmanlı olarak oluştur
- [ ] `venue` şemasındaki tüm tabloları EF Core entity olarak tanımla
- [ ] Migration oluştur ve Neon.tech'e uygula
- [ ] Repository'leri yaz: `IVenueRepository`, `IDistrictRepository`, `IConceptTagRepository`
- [ ] İstanbul ilçe seed data'sını migration ile ekle (39 ilçe)
- [ ] Sistem konsept etiketlerini seed data olarak ekle (doğum günü, romantik, manzaralı, açık hava, kahvaltı, canlı müzik, vb. — min. 20 etiket)

**Endpoint'ler:**

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/v1/venues` | Tüm mekânları listele (sayfalı) |
| GET | `/api/v1/venues/{id}` | Mekân detayı |
| POST | `/api/v1/venues` | Yeni mekân ekle (admin) |
| PUT | `/api/v1/venues/{id}` | Mekân güncelle (admin) |
| DELETE | `/api/v1/venues/{id}` | Mekân sil (admin) |
| GET | `/api/v1/districts` | İlçe listesi |
| GET | `/api/v1/concept-tags` | Konsept etiket listesi |
| POST | `/api/v1/concept-tags` | Yeni etiket ekle (admin) |

**Test kriterleri:**
- Swagger ile mekân ekleniyor, listeleniyor, siliniyor
- 39 İstanbul ilçesi seed data'dan geliyor
- Admin token olmadan POST/PUT/DELETE 403 dönüyor

---

#### ADIM 4 — Search & Filter Service Backend

> **Hedef:** Kullanıcının ilçe + konsept kombinasyonu ile mekân arayabildiği servis çalışır hale gelsin.

**Yapılacaklar:**

- [ ] `SpotFinder.SearchService` solution'ını 3 katmanlı olarak oluştur
- [ ] Venue Service veritabanına read-only bağlantı (aynı Neon.tech, `venue` şeması)
- [ ] İlçe bazlı filtreleme sorgusu
- [ ] Çoklu konsept etiketi bazlı filtreleme sorgusu (AND mantığı)
- [ ] Sıralama: puan (yüksekten düşüğe), yenilik (yeni eklenen önce)
- [ ] Sayfalama (page + pageSize)

**Endpoint'ler:**

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/v1/search/venues` | Filtreli mekân arama |
| GET | `/api/v1/search/venues/featured` | Öne çıkan mekânlar (anasayfa) |

**Query parametreleri (`/search/venues`):**
```
districtId    int?      İlçe ID
conceptTagIds int[]?    Konsept etiket ID listesi (çoklu)
sortBy        string?   rating | newest (default: rating)
page          int       default: 1
pageSize      int       default: 20, max: 50
```

**Test kriterleri:**
- `districtId=1&conceptTagIds=2,5` sorgusu doğru mekânları döndürüyor
- Sonuçlar puana göre sıralanıyor
- Hiç mekân yoksa boş liste + 200 dönüyor

---

#### ADIM 5 — Review Service Backend

> **Hedef:** Kullanıcı yorumları ve puanlama sistemi çalışır hale gelsin.

**Yapılacaklar:**

- [ ] `SpotFinder.ReviewService` solution'ını 3 katmanlı olarak oluştur
- [ ] `review` şemasındaki tabloları EF Core entity olarak tanımla
- [ ] Migration oluştur ve Neon.tech'e uygula
- [ ] Yorum eklendiğinde Venue Service'teki `average_rating` ve `review_count` güncelleme mekanizması (HTTP call veya event)
- [ ] Moderasyon durumu akışı: `pending → approved / rejected`

**Endpoint'ler:**

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/v1/reviews` | Yorum ekle (JWT gerekli) |
| GET | `/api/v1/reviews/venue/{venueId}` | Mekânın onaylı yorumları |
| GET | `/api/v1/reviews/pending` | Bekleyen yorumlar (admin) |
| PUT | `/api/v1/reviews/{id}/approve` | Yorumu onayla (admin) |
| PUT | `/api/v1/reviews/{id}/reject` | Yorumu reddet (admin) |
| DELETE | `/api/v1/reviews/{id}` | Yorum sil (admin) |

**Test kriterleri:**
- Yorum eklendikten sonra `review.reviews` tablosunda `status = 'pending'` görünüyor
- Admin onayladıktan sonra `/venue/{venueId}` sorgusunda geliyor
- JWT olmadan POST 401 dönüyor

---

#### ADIM 6 — Kullanıcı Etkileşim Endpoint'leri (Favori, Ziyaret, Paylaşım)

> **Hedef:** Kullanıcının mekânı favorilere ekleyebildiği, ziyaret geçmişini tutabildiği endpoint'ler çalışır hale gelsin.

**Yapılacaklar:**

- [ ] Auth Service'e `user_favorites` ve `user_visits` endpoint'leri ekle
- [ ] Deep link şeması tanımla: `spotfinder://venue/{id}`
- [ ] Paylaşım için `venue` detayında `shareUrl` alanı döndür

**Endpoint'ler (Auth Service'e eklenir):**

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/v1/users/favorites/{venueId}` | Favoriye ekle |
| DELETE | `/api/v1/users/favorites/{venueId}` | Favoriden çıkar |
| GET | `/api/v1/users/favorites` | Favori mekân listesi |
| POST | `/api/v1/users/visits/{venueId}` | Ziyaret olarak işaretle |
| GET | `/api/v1/users/visits` | Ziyaret geçmişi |

**Test kriterleri:**
- Favori ekleme/çıkarma `auth.user_favorites` tablosunu güncelliyor
- Aynı mekânı iki kez favoriye eklemek 409 dönüyor
- JWT olmadan tüm endpoint'ler 401 dönüyor

---

#### ADIM 7 — Admin Paneli (Next.js Web Uygulaması)

> **Hedef:** Mekân ve etiket yönetimi için tam işlevsel bir web admin paneli çalışır hale gelsin.

**Yapılacaklar:**

- [ ] `spotfinder-admin` Next.js projesi oluştur (App Router, TypeScript)
- [ ] Admin login sayfası (JWT ile)
- [ ] Dashboard sayfası (toplam mekân, kullanıcı, yorum sayısı)
- [ ] Mekân listesi sayfası (tablo, arama, sayfalama)
- [ ] Mekân ekleme / düzenleme formu
- [ ] Fotoğraf yükleme (menü + mekân görseli) — S3 presigned URL veya base64
- [ ] Konsept etiketi yönetim sayfası (sistem + custom)
- [ ] Yorum moderasyon sayfası (bekleyen yorumları onayla/reddet)
- [ ] Kullanıcı listesi sayfası

**Sayfalar:**

| Sayfa | Route | Açıklama |
|---|---|---|
| Login | `/login` | Admin girişi |
| Dashboard | `/dashboard` | Özet istatistikler |
| Mekânlar | `/venues` | Liste + arama |
| Mekân Ekle/Düzenle | `/venues/new`, `/venues/[id]` | Form |
| Etiketler | `/concept-tags` | Etiket yönetimi |
| Yorumlar | `/reviews` | Moderasyon |
| Kullanıcılar | `/users` | Liste |

**Test kriterleri:**
- Admin login çalışıyor, token localStorage'a kaydediliyor
- Mekân ekleme formu Venue Service'e POST atıyor, Neon.tech'e kaydediliyor
- Fotoğraf yükleme çalışıyor ve URL venue kaydına bağlanıyor
- Onaylanmamış JWT ile `/dashboard` → `/login`'e yönlendiriyor

---

#### ADIM 8 — Flutter Mobil Uygulama İskeleti ve Auth Ekranları

> **Hedef:** Flutter uygulaması başlatılıyor, navigasyon kurulu, login/register ekranları Auth Service'e bağlı.

**Yapılacaklar:**

- [ ] `spotfinder_app` Flutter projesi oluştur
- [ ] Paketleri ekle: `flutter_bloc`, `dio`, `go_router`, `flutter_localizations`, `hive`, `google_sign_in`, `sign_in_with_apple`
- [ ] Tema tanımı (Material 3, renk paleti, font)
- [ ] TR / EN çoklu dil desteği (`app_tr.arb`, `app_en.arb`)
- [ ] Temel navigasyon yapısı (`go_router` ile route tanımları)
- [ ] `AuthBloc` ve `AuthRepository` yaz
- [ ] Token yönetimi: Hive ile güvenli saklama, uygulama açılışında token kontrolü

**Ekranlar:**

| Ekran | Route | Açıklama |
|---|---|---|
| Splash | `/` | Token kontrolü, yönlendirme |
| Onboarding | `/onboarding` | İlk açılış tanıtım |
| Login | `/login` | E-posta + şifre, Google, Apple |
| Register | `/register` | Kayıt formu |
| OTP Doğrulama | `/otp` | SMS kodu girişi |

**Test kriterleri:**
- Uygulama açılıyor, splash screen token kontrolü yapıyor
- Login ekranından e-posta + şifre ile giriş çalışıyor, token Hive'a kaydediliyor
- Google Sign-In butonu çalışıyor (Auth Service `/google` endpoint'ine bağlanıyor)
- Giriş yapıldıktan sonra ana sayfaya yönlendiriyor

---

#### ADIM 9 — Flutter Mobil: Keşif, Arama ve Mekân Detay Ekranları

> **Hedef:** Kullanıcı mekânları keşfedebiliyor, filtreleyebiliyor, detay sayfasını görebiliyor.

**Yapılacaklar:**

- [ ] `SearchBloc`, `VenueBloc`, `SearchRepository`, `VenueRepository` yaz
- [ ] Ana sayfa: öne çıkan mekânlar kartları
- [ ] Filtre ekranı: ilçe seçimi + çoklu konsept etiketi seçimi
- [ ] Arama sonuçları listesi (kart görünümü, infinite scroll)
- [ ] Mekân detay sayfası: fotoğraf galerisi, menü fotoğrafları, otopark durumu, çalışma saatleri
- [ ] Google Maps entegrasyonu: mini harita widget + "Yol Tarifi Al" butonu
- [ ] Favori butonu (kalp ikonu) — FavoriteBloc ile bağlı
- [ ] Paylaşım butonu — deep link üretiyor

**Ekranlar:**

| Ekran | Route | Açıklama |
|---|---|---|
| Ana Sayfa | `/home` | Öne çıkan mekânlar |
| Keşfet / Filtre | `/explore` | İlçe + konsept filtresi |
| Arama Sonuçları | `/search` | Filtrelenmiş mekân listesi |
| Mekân Detay | `/venue/:id` | Detay sayfası |
| Favorilerim | `/favorites` | Favori mekânlar |
| Geçmiş Ziyaretler | `/visits` | Ziyaret geçmişi |

**Test kriterleri:**
- Filtre ekranından ilçe + etiket seçildiğinde Search Service'e doğru query atılıyor
- Mekân kartına tıklandığında detay sayfası açılıyor
- Google Maps mekânın koordinatlarını gösteriyor
- Favori butonu çalışıyor, kullanıcı çıkıp girse bile favori bilgisi korunuyor

---

#### ADIM 10 — Flutter Mobil: Yorumlar, Push Bildirimler ve Son Düzenlemeler

> **Hedef:** Yorum sistemi ve push bildirimler çalışıyor, uygulama MVP olarak yayına hazır.

**Yapılacaklar:**

- [ ] `ReviewBloc` ve `ReviewRepository` yaz
- [ ] Mekân detay sayfasına yorum listesi ve yorum yazma formu ekle
- [ ] Profil sayfası (kullanıcı adı, avatar, favori sayısı, ziyaret sayısı)
- [ ] Firebase Cloud Messaging (FCM) entegrasyonu
- [ ] FCM token'ı Auth Service'e kaydet
- [ ] Bildirim izni alma akışı (iOS + Android)
- [ ] Uygulama arka planda ve kapalıyken bildirim alma
- [ ] Dil değiştirme ayarı (TR / EN toggle)
- [ ] Genel hata yönetimi: internet bağlantısı yok, sunucu hatası ekranları
- [ ] Uygulama ikonu ve splash screen görseli

**Ekranlar:**

| Ekran | Route | Açıklama |
|---|---|---|
| Profil | `/profile` | Kullanıcı bilgileri, ayarlar |
| Yorum Yaz | `/venue/:id/review` | Puan + yorum formu |
| Ayarlar | `/settings` | Dil, bildirim tercihleri |
| Hata | — | Bağlantı hatası ekranı |

**Test kriterleri:**
- Yorum yazılıyor, Review Service'e kaydediliyor, admin panelde `pending` görünüyor
- Admin onayladıktan sonra mekân detay sayfasında yorum görünüyor
- Push bildirim iOS ve Android'de alınıyor
- Uygulama Türkçe/İngilizce arasında geçiş yapabiliyor
- Tüm ekranlar internet olmadan anlamlı hata mesajı gösteriyor

---

### Adım Geçiş Protokolü

Her adım tamamlandığında şu kontrol listesini uygula:

```
✅ Adım X tamamlandı kontrol listesi:

□ Tüm endpoint'ler / ekranlar test kriterleri listesindeki her maddeyi geçiyor
□ Neon.tech veritabanında ilgili tablolar doğru verilerle doluyor
□ Swagger UI (backend) veya Flutter (mobil) üzerinden manuel test yapıldı
□ Revize isteği varsa Claude'a ilet: "Adım X - [revize konusu]"
□ Tüm revizyonlar tamamlandı ve tekrar test edildi
□ Bir sonraki adıma geçmeye hazır
```

---

### Faz 2 — Venue Owner Portalı

- `venue_owner` rolü ve auth sistemi
- Venue owner mobil/web girişi
- Kendi mekânlarının bilgilerini güncelleme
- Menü fotoğrafı yükleme
- Kampanya ve duyuru oluşturma
- Mekân istatistikleri

### Faz 3 — Büyüme

- Ankara, İzmir genişlemesi
- Elasticsearch ile gelişmiş arama
- AI destekli kişiselleştirilmiş öneri motoru
- Rezervasyon / ön sipariş entegrasyonu
- Gamification sistemi

---

## 8. Geliştirme Ortamı

| Kategori | Araç / Versiyon |
|---|---|
| İşletim Sistemi | Windows 11 |
| IDE | Cursor (birincil), Visual Studio 2022 |
| .NET | .NET 9 (kurulu) |
| Flutter | Kurulu (PATH sorunu var, düzeltilecek) |
| Docker | Docker Desktop 24.x (kurulu) |
| Node.js | v20.x (kurulu) |
| Veritabanı (Dev) | Neon.tech — bağlantı Bölüm 5.4.2'de |
| Versiyon Kontrol | Git + GitHub |
| API Test | Postman / Swagger UI |
| CI/CD | GitHub Actions → ECR → ECS |

---

## 9. Klasör Yapısı (Monorepo)

```
spotfinder/
├── docs/
│   └── SpotFinder_Project_Document.md
├── backend/
│   ├── SpotFinder.AuthService/
│   │   ├── SpotFinder.AuthService.API/
│   │   ├── SpotFinder.AuthService.Business/
│   │   ├── SpotFinder.AuthService.Data/
│   │   └── SpotFinder.AuthService.sln
│   ├── SpotFinder.VenueService/
│   │   ├── SpotFinder.VenueService.API/
│   │   ├── SpotFinder.VenueService.Business/
│   │   ├── SpotFinder.VenueService.Data/
│   │   └── SpotFinder.VenueService.sln
│   ├── SpotFinder.SearchService/
│   │   ├── SpotFinder.SearchService.API/
│   │   ├── SpotFinder.SearchService.Business/
│   │   ├── SpotFinder.SearchService.Data/
│   │   └── SpotFinder.SearchService.sln
│   ├── SpotFinder.ReviewService/
│   │   ├── SpotFinder.ReviewService.API/
│   │   ├── SpotFinder.ReviewService.Business/
│   │   ├── SpotFinder.ReviewService.Data/
│   │   └── SpotFinder.ReviewService.sln
│   └── docker-compose.yml
├── mobile/
│   └── spotfinder_app/
├── admin-panel/
│   └── spotfinder-admin/
└── infrastructure/
    └── terraform/
```

---

## 10. AI Destekli Geliştirme — Prompt Kılavuzu

> **Bu doküman her AI oturumunun başında context olarak verilmelidir.**  
> AI bu dokümandaki kurallara uymakla yükümlüdür.

### 10.1 Her Oturum Başına Eklenecek Kural Bloğu

Her yeni AI oturumunda bu bloğu promptun başına yapıştır:

```
Aşağıdaki kurallar SpotFinder projesi için kesinlikle geçerlidir.
Bu doküman projenin tek kaynak of truth'udur, dokümana aykırı kod yazma.

ZORUNLU KURALLAR:
1. .NET 9, CQRS (MediatR), EF Core 9, PostgreSQL kullan
2. Her servis 3 katman: [Servis].API / [Servis].Business / [Servis].Data
3. Bağımlılık yönü: API → Business → Data (ters referans yasak)
4. Business: Command/Query + Handler + Validator AYNI dosyada
5. Data: Entity + IRepository interface + Repository implementasyon AYRI dosyalarda
6. Controller iş mantığı içermez — yalnızca _mediator.Send() çağırır
7. Tüm metodlar async/await + CancellationToken kullanır
8. Validasyon FluentValidation ile yapılır, [Required] gibi DataAnnotations yasak
9. Veritabanı şema prefix'leri: auth.* / venue.* / review.*
10. Connection string: Host=ep-bitter-cloud-a9xk9knc-pooler.gwc.azure.neon.tech;
    Port=5432;Database=neondb;Username=neondb_owner;
    Password=npg_NamhpWnicC82;SSL Mode=Require;Trust Server Certificate=true
```

### 10.2 Yeni Servis Oluşturma Promptu

```
SpotFinder projesi için [SERVİS_ADI] mikroservisini oluştur.

Katman yapısı:
- SpotFinder.[SERVİS_ADI].API       (Web API, .NET 9)
- SpotFinder.[SERVİS_ADI].Business  (Class Library)
- SpotFinder.[SERVİS_ADI].Data      (Class Library)

Referans sırası: API → Business → Data

Connection string (appsettings.json):
Host=ep-bitter-cloud-a9xk9knc-pooler.gwc.azure.neon.tech;Port=5432;
Database=neondb;Username=neondb_owner;Password=npg_NamhpWnicC82;
SSL Mode=Require;Trust Server Certificate=true

PostgreSQL şema adı: [ŞEMA_ADI]

Oluşturulacaklar:
- Entity sınıfları (Data/Entities/)
- AppDbContext — OnModelCreating ile şema ve tablo mapping (Data/Context/)
- Repository interface ve implementasyonları (Data/Repositories/)
- İlk EF Core migration
- Command/Query/Handler/Validator dosyaları (Business/)
- Gerekli servis interface ve implementasyonları (Business/Services/)
- Controller (API/Controllers/)
- Program.cs — DI kayıtları, JWT, Swagger
- appsettings.json

Şema ve tablolar:
[İLGİLİ ŞEMA BLOĞUNU BURAYA KOPYALA — Bölüm 5.4.3'ten]
```

### 10.3 Mevcut Servise Yeni Endpoint Ekleme Promptu

```
SpotFinder.[SERVİS_ADI] servisine [ENDPOINT_ADI] endpoint'i ekle.

Eklenecekler:
- [Command/Query] + Handler + Validator (Business/Commands/ veya Queries/)
- Controller action (API/Controllers/)
- Gerekiyorsa yeni Repository metodu (Data/Repositories/)

HTTP Metodu: [GET/POST/PUT/DELETE]
Route: [/api/v1/...]
Request: { [alanlar] }
Response: { [alanlar] }

İş kuralları:
[KURALLARI BURAYA YAZ]
```

### 10.4 Flutter Ekran Oluşturma Promptu

```
SpotFinder Flutter uygulaması için [EKRAN_ADI] ekranını oluştur.

Kurallar:
- State management: Bloc
- HTTP istemcisi: Dio
- Dil desteği: TR ve EN (flutter_localizations)
- Tasarım: Material 3, temiz ve modern UI
- Hata yönetimi: try/catch + SnackBar ile kullanıcıya göster

API endpoint: [HTTP_METODU] [BASE_URL]/[ENDPOINT]
Request: { [alanlar] }
Response: { [alanlar] }

Ekran gereksinimleri:
[EKRANDAKİ ÖZELLİKLERİ BURAYA YAZ]
```

### 10.5 Migration Komutları (Referans)

```bash
# Migration oluştur
dotnet ef migrations add [MigrationAdı] \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API

# Veritabanına uygula (Neon.tech)
dotnet ef database update \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API

# Son migration'ı geri al
dotnet ef migrations remove \
  --project SpotFinder.[Servis].Data \
  --startup-project SpotFinder.[Servis].API
```

---

## 11. Sonraki Adımlar

1. GitHub'da `spotfinder` repo oluştur, klasör yapısını kur, bu MD dosyasını `docs/` altına ekle
2. Cursor'ı aç, bu MD dosyasını context olarak ver, Bölüm 10.1 kurallarını promptun başına ekle
3. Auth Service solution iskeletini oluştur (Bölüm 10.2 promptunu kullan)
4. Neon.tech'e ilk migration'ı uygula — `auth` şeması oluşur
5. Postman ile Register ve Login endpoint'lerini test et
6. Venue Service geliştirmeye başla
7. Flutter uygulama iskeletini oluştur (navigation, theme, localization)
8. Admin Panel iskeletini oluştur (Next.js)
9. AWS altyapısını Terraform ile kodla (production öncesi)
10. CI/CD pipeline'ını GitHub Actions ile kur

---

*Bu doküman yaşayan bir belgedir. Her geliştirme oturumunda güncellenecektir.*
