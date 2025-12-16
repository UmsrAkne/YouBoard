namespace YouBoard.Models.Request
{
    public static class IssueCustomFieldFactory
    {
        /// <summary>
        /// 時間を格納するカスタムフィールドの値を更新するためのリクエストペイロードを生成します。
        /// </summary>
        /// <param name="name">更新対象のフィールド名。</param>
        /// <param name="minutes">更新する数値を分単位で入力します。</param>
        /// <returns>引数を元に生成されたペイロードを返します。</returns>
        public static object Period(string name, int minutes)
        {
            // カスタムフィールドの配列をルートに配置し、その中に IssueCustomFields を入れたオブジェクト
            return new
            {
                customFields = new[]
                {
                    new IssueCustomFields
                    {
                        Name = name,
                        Type = "PeriodIssueCustomField",
                        Value = new { minutes, },
                    },
                },
            };
        }
    }
}