using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Configs
{
    public class BaseEnvironmentConfig<T> : Config<T> where T : class
    {
        enum Environment
        {
            Local,
            Dev,
            Development,
            UnitTest,
            QA,
            AT,
            Stage,
            Test,
            Prod,
            Production
        }
        Environment? _EnvironmentName;
        Environment EnvironmentName
        {
            get
            {
                if (_EnvironmentName == null)
                    _EnvironmentName = Name.ToEnum<Environment>();

                return _EnvironmentName.Value;
            }
        }

        public string Name { get; set; }

        bool? _IsLocal;
        public bool IsLocal
        {
            get
            {
                if (_IsLocal == null)
                    _IsLocal = 
                        EnvironmentName == Environment.Local || 
                        EnvironmentName == Environment.Dev ||
                        EnvironmentName == Environment.Development;

                return _IsLocal.Value;
            }
        }

        bool? _IsProd;
        public bool IsProd
        {
            get
            {
                if (_IsProd == null)
                    _IsProd =
                        EnvironmentName == Environment.Prod ||
                        EnvironmentName == Environment.Production;

                return _IsProd.Value;
            }
        }
    }
}