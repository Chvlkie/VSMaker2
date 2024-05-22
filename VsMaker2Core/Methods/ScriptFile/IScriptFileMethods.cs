using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public interface IScriptFileMethods
    {
        ScriptFileData GetScriptFileData(int scriptFileId);
        (bool Success, string ErrorMessage) WriteScriptData(ScriptFileData scriptFileData);
        #region Get

        #endregion Get

        #region Read

        #endregion Read
    }
}