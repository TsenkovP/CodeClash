# CodeClash 🚀

**CodeClash** е уеб платформа за организиране и управление на състезания по програмиране, хакатони и ИТ олимпиади. Приложението управлява цялостния състезателен процес: обявяване на събития, регистрация на ученически отбори, предаване на разработки с връзки към GitHub, оценяване от жури и класиране в реално време.

---

## 🛠️ Технологичен стек

* **Платформа:** .NET 9 (ASP.NET Core MVC)
* **ORM:** Entity Framework Core
* **База данни:** Microsoft SQL Server
* **Потребителски интерфейс:** Bootstrap 5, Razor Views, Custom CSS Micro-Cards
* **Среда за разработка:** JetBrains Rider / Visual Studio на macOS & Windows
* **Контрол на версиите:** Git & GitHub

---

## 📌 Основни модули и функционалности

### 1. Състезания (`Competitions`)

* Пълен CRUD цикъл: създаване, преглед, редакция и изтриване на събития.
* Контрол на периода за записване чрез флаг `IsOpenForRegistration`.
* Задаване на регламент, дата на провеждане и лимит на състезателите в отбор (`MaxTeamMembers`).
* Модерен картов интерфейс с отделни рамки за параметрите и брой записани отбори.

### 2. Отбори (`Teams`)

* Регистрация на отбори с ментор/ръководител и контактен имейл.
* Директно записване от детайлната страница на конкретно състезание с предварително попълнени данни.
* Сървърна защита срещу записване в затворени състезания.
* Валидация за уникалност на името на отбора в рамките на едно състезание.

### 3. Предадени проекти (`ProjectSubmissions`)

* Прикачване на разработка (заглавие и валидиран GitHub URL адрес) към регистриран отбор.
* Бизнес правило: всеки отбор може да предаде само един финален проект.
* Панел за оценяване от жури по скала от 0 до 100 точки.

### 4. Класиране (`Leaderboard`)

* Автоматично класиране на отборите по получените точки в низходящ ред.
* Интерактивно филтриране по състезание през падащо меню.
* Визуални баджове за челната тройка (🥇 1-во, 🥈 2-ро, 🥉 3-то място).
* Бързи връзки към GitHub хранилищата на прегледаните проекти.

---

## 🏗️ Архитектура на базата данни

```text
[ Competitions ] 1 ───────< N [ Teams ] 1 ─────── 1 [ ProjectSubmissions ]

```

* **Competitions:** `Id`, `Title`, `Description`, `EventDate`, `IsOpenForRegistration`, `MaxTeamMembers`
* **Teams:** `Id`, `TeamName`, `MentorName`, `ContactEmail`, `RegisteredOn`, `MaxTeamMembers`, `CompetitionId`
* **ProjectSubmissions:** `Id`, `ProjectTitle`, `GitHubUrl`, `SubmittedOn`, `Score`, `TeamId`

---

## ⚙️ Инсталация и локално стартиране

### Предварителни изисквания

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* Microsoft SQL Server (локална инсталация или Docker контейнер)
* [JetBrains Rider](https://www.jetbrains.com/rider/) или [Visual Studio](https://visualstudio.microsoft.com/)

### Инструкции за пускане

1. **Клониране на хранилището:**
```bash
git clone https://github.com/<your-username>/CodeClash.git
cd CodeClash

```


2. **Настройка на връзката с базата данни:**

### Настройка на базата данни (appsettings.json)

По подразбиране проектът е настроен за локален **SQL Server / LocalDB**:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CodeClashDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```


3. **Стартиране на базата данни:**
   Уверете се, че базата `CodeClashDb` съществува и таблиците са създадени според схемата на моделите.
4. **Възстановяване на пакетите и стартиране на проекта:**
```bash
dotnet restore
dotnet build
dotnet run --project CodeClash

```


5. **Отворете в браузър:**
   Навигирайте до адреса, изписан в конзолата (по подразбиране: `http://localhost:5064`).

---

## 📋 План за бъдещо надграждане

* [ ] Управление на роли и права чрез ASP.NET Core Identity (Администратор, Жури, Ръководител на отбор).
* [ ] Обобщено информационно табло на началната страница (`Home Dashboard`).
* [ ] Автоматизирана проверка на GitHub хранилищата през GitHub REST API.
* [ ] Експорт на протоколите от класирането в PDF и Excel формат.

---

## 📄 Лиценз

Проектът се разпространява под лиценз MIT.