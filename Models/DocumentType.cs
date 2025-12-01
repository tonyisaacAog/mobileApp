namespace CompanyApi.Models
{
    public enum DocumentType
    {
        I,
        C,
        D,
        SR,
        RR,
        UnKnown
    }
    public static class DocumentTypeMapper
    {
        public static DocumentType MapApiCodeToEnum(string apiCode)
        {

            switch (apiCode.ToLower())
            {
                case "i":
                    return DocumentType.I;
                case "c":
                    return DocumentType.C;
                case "d":
                    return DocumentType.D;
                case "sr":
                    return DocumentType.SR;
                case "rr":
                    return DocumentType.RR;
                default:
                    return DocumentType.UnKnown;
            }
        }
    }
    public static class DocumentTypeTitleMapper
    {
        public static string GetTitleForEnum(DocumentType documentType)
        {
            switch (documentType)
            {
                case DocumentType.I:
                    return "I";
                case DocumentType.C:
                    return "C";
                case DocumentType.D:
                    return "D";
                case DocumentType.SR:
                    return "SR";
                case DocumentType.RR:
                    return "RR";
                default:
                    return DocumentType.UnKnown.ToString();
            }
        }
    }
    public static class DocumentTypeFilterWithCondition
    {
        public static List<DocumentType> GetDocumentTypeWithAppSetting(bool isReceipt, bool isInvoice)
        {

            return
                isReceipt && !isInvoice ?
                new List<DocumentType> { DocumentType.SR, DocumentType.RR } :
                !isReceipt && isInvoice ?
                new List<DocumentType> { DocumentType.I, DocumentType.C, DocumentType.D } :
                new List<DocumentType> { DocumentType.SR, DocumentType.RR, DocumentType.I, DocumentType.C, DocumentType.D };
        }
    }
}