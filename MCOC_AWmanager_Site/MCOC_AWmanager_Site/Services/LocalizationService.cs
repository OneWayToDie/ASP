namespace MCOC_AWmanager_Site.Services;

public class LocalizationService
{
    private string _currentLanguage = "ru";

    public string CurrentLanguage => _currentLanguage;
    public bool IsRu => _currentLanguage == "ru";
    public bool IsEn => _currentLanguage == "en";

    public event Action? OnLanguageChanged;

    public void SetLanguage(string lang)
    {
        if (_currentLanguage != lang)
        {
            _currentLanguage = lang;
            OnLanguageChanged?.Invoke();
        }
    }

    public void ToggleLanguage()
    {
        SetLanguage(_currentLanguage == "ru" ? "en" : "ru");
    }

    // Navigation
    public string NavHome => IsRu ? "Главная" : "Home";
    public string NavFeatures => IsRu ? "Возможности" : "Features";
    public string NavDemo => IsRu ? "Демо" : "Demo";
    public string NavChampions => IsRu ? "Чемпионы" : "Champions";
    public string NavFaq => IsRu ? "FAQ" : "FAQ";
    public string NavContacts => IsRu ? "Контакты" : "Contacts";

    // Home page
    public string HomeBadge => IsRu ? "MARVEL CONTEST OF CHAMPIONS" : "MARVEL CONTEST OF CHAMPIONS";
    public string HomeTitle => IsRu ? "MCOC AW Manager" : "MCOC AW Manager";
    public string HomeSubtitle => IsRu
        ? "Полный контроль над вашим альянсом в Marvel Contest of Champions. Управляйте игроками, планируйте военные кампании и побеждайте."
        : "Full control over your alliance in Marvel Contest of Champions. Manage players, plan war campaigns and conquer.";
    public string HomeDownloadBtn => IsRu ? "Скачать бесплатно" : "Download Free";
    public string HomeLearnMore => IsRu ? "Узнать больше" : "Learn more";
    public string HomeStatChampions => IsRu ? "Чемпионов" : "Champions";
    public string HomeStatNodes => IsRu ? "Узлов карты" : "Map nodes";
    public string HomeStatMembers => IsRu ? "Участников" : "Members";

    public string HomeDownloadTitle => IsRu ? "Скачать MCOC AW Manager" : "Download MCOC AW Manager";
    public string HomeDownloadDesc => IsRu
        ? "Бесплатное приложение для Windows. Быстрая установка, автоматические обновления."
        : "Free application for Windows. Quick installation, automatic updates.";
    public string HomeDownloadBtnLarge => IsRu ? "Скачать последнюю версию" : "Download Latest Version";

    public string HomeWhyTitle => IsRu ? "Почему MCOC AW Manager?" : "Why MCOC AW Manager?";
    public string HomeWhySubtitle => IsRu
        ? "Мощные инструменты для управления альянсом, созданные игроками для игроков"
        : "Powerful alliance management tools, built by players for players";

    public string HomeFeature1Title => IsRu ? "Военная карта" : "War Map";
    public string HomeFeature1Desc => IsRu
        ? "Интерактивная карта на 50 узлов с советами по защите и авто-расстановкой чемпионов"
        : "Interactive 50-node map with defense tips and auto-champion placement";
    public string HomeFeature2Title => IsRu ? "Реалтайм чат" : "Realtime Chat";
    public string HomeFeature2Desc => IsRu
        ? "Мгновенный обмен сообщениями с участниками альянса, прикрепление файлов"
        : "Instant messaging with alliance members, file attachments";
    public string HomeFeature3Title => IsRu ? "Энциклопедия" : "Encyclopedia";
    public string HomeFeature3Desc => IsRu
        ? "База данных 200+ чемпионов с полными описаниями способностей и тегами"
        : "Database of 200+ champions with full ability descriptions and tags";
    public string HomeFeature4Title => IsRu ? "Статистика" : "Statistics";
    public string HomeFeature4Desc => IsRu
        ? "Отслеживание убийств, смерти, рейтинга участников с настраиваемой формулой"
        : "Track kills, deaths, member ratings with customizable formula";
    public string HomeFeature5Title => IsRu ? "Новости MCOC" : "MCOC News";
    public string HomeFeature5Desc => IsRu
        ? "Автоматический парсинг свежих новостей из официальных источников"
        : "Automatic parsing of fresh news from official sources";
    public string HomeFeature6Title => IsRu ? "Гибкие настройки" : "Flexible Settings";
    public string HomeFeature6Desc => IsRu
        ? "Тёмная тема, звуки уведомлений, напоминания о войнах"
        : "Dark theme, notification sounds, war reminders";

    // Footer
    public string FooterTagline => IsRu ? "Полный контроль над вашим альянсом" : "Full control over your alliance";
    public string FooterContacts => IsRu ? "Контакты" : "Contacts";
    public string FooterCopyright => IsRu ? "© 2026 MCOC AW Manager. Все права защищены." : "© 2026 MCOC AW Manager. All rights reserved.";

    // Features page
    public string FeaturesTitle => IsRu ? "Возможности" : "Features";
    public string FeaturesSubtitle => IsRu
        ? "Всё, что нужно для управления альянсом, в одном приложении"
        : "Everything you need for alliance management in one application";

    // Demo page
    public string DemoTitle => IsRu ? "Как начать" : "How to Start";
    public string DemoSubtitle => IsRu
        ? "Пошаговое руководство по использованию MCOC AW Manager"
        : "Step-by-step guide to using MCOC AW Manager";
    public string DemoStep1Title => IsRu ? "Скачайте и установите" : "Download and Install";
    public string DemoStep1Desc => IsRu
        ? "Скачайте последнюю версию MCOC AW Manager с главной страницы. Установка занимает менее минуты. Программа работает на Windows 10/11."
        : "Download the latest version of MCOC AW Manager from the home page. Installation takes less than a minute. Works on Windows 10/11.";
    public string DemoStep2Title => IsRu ? "Создайте аккаунт" : "Create Account";
    public string DemoStep2Desc => IsRu
        ? "Запустите приложение и нажмите \"Регистрация\". Укажите никнейм, email и пароль. Ваши данные надёжно хранятся в зашифрованном облаке."
        : "Launch the app and click \"Register\". Enter your nickname, email and password. Your data is securely stored in encrypted cloud.";
    public string DemoStep3Title => IsRu ? "Создайте альянс" : "Create Alliance";
    public string DemoStep3Desc => IsRu
        ? "Создайте новый альянс или вступите в существующий. Установите тег, пригласите участников через систему заявок."
        : "Create a new alliance or join an existing one. Set a tag, invite members through the request system.";
    public string DemoStep4Title => IsRu ? "Настройте военную карту" : "Set Up War Map";
    public string DemoStep4Desc => IsRu
        ? "Используйте авто-расстановку или расставляйте чемпионов вручную. Советы по защите подскажут оптимальных чемпионов для каждого узла."
        : "Use auto-placement or place champions manually. Defense tips will suggest optimal champions for each node.";
    public string DemoStep5Title => IsRu ? "Побеждайте!" : "Conquer!";
    public string DemoStep5Desc => IsRu
        ? "Координируйте действия через чат, отслеживайте статистику, анализируйте результаты и становитесь сильнее с каждым сезоном."
        : "Coordinate through chat, track statistics, analyze results and become stronger with each season.";

    public string DemoScreenshotsTitle => IsRu ? "Скриншоты" : "Screenshots";
    public string DemoScreenshotsSubtitle => IsRu ? "Как выглядит приложение" : "How the application looks";
    public string DemoReqTitle => IsRu ? "Системные требования" : "System Requirements";

    // Champions page
    public string ChampionsTitle => IsRu ? "Энциклопедия чемпионов" : "Champion Encyclopedia";
    public string ChampionsSubtitle => IsRu
        ? "200+ чемпионов с полными описаниями, способностями и тегами"
        : "200+ champions with full descriptions, abilities and tags";

    // FAQ page
    public string FaqTitle => IsRu ? "Часто задаваемые вопросы" : "Frequently Asked Questions";
    public string FaqSubtitle => IsRu
        ? "Ответы на популярные вопросы о MCOC AW Manager"
        : "Answers to popular questions about MCOC AW Manager";

    // Contacts page
    public string ContactsTitle => IsRu ? "Контакты" : "Contacts";
    public string ContactsSubtitle => IsRu ? "Свяжитесь с автором проекта" : "Contact the project author";
}
