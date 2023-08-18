using System.Runtime.Serialization;

namespace MP3_Tag_Editor.Exceptions;

[Serializable]
public class InvalidId3PropertyException : Exception
{

    public InvalidId3PropertyException(string propertyName)
        : base($"The property '{propertyName}' is not a valid Id3 property.")
    {
    }

    public InvalidId3PropertyException(string propertyName, Exception innerException)
        : base($"The property '{propertyName}' is not a valid Id3 property.", innerException)
    {
    }

    public InvalidId3PropertyException(string propertyName, string message)
        : base($"The property '{propertyName}' is not a valid Id3 property. {message}")
    {
    }

    public InvalidId3PropertyException(string propertyName, string message, Exception innerException)
        : base($"The property '{propertyName}' is not a valid Id3 property. {message}", innerException)
    {
    }

    protected InvalidId3PropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}