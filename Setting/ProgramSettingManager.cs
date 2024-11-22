namespace SnaptagOwnKioskInternalBackend.Setting
{
    using System.Text.Json;

    public class ProgramSettingManager
    {
        private readonly string _configFilePath = "programsettings.json";
        private static readonly Lazy<ProgramSettingManager> _instance =
            new Lazy<ProgramSettingManager>(() => new ProgramSettingManager());
        public ProgramSetting Settings { get; private set; }

        // 외부에서 인스턴스를 가져오는 속성
        public static ProgramSettingManager Instance => _instance.Value;

        // private 생성자
        private ProgramSettingManager()
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                Settings = JsonSerializer.Deserialize<ProgramSetting>(json) ?? new ProgramSetting();
            }
            else
            {
                Settings = new ProgramSetting(); // 기본 설정 생성
                SaveSettings(); // 설정 파일 생성
            }
        }

        public void SaveSettings()
        {
            var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, json);
        }
    }

}
