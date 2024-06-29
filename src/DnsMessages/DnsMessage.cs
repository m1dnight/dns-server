using System.Collections;

namespace codecrafters_dns_server.DnsMessages;

public class DnsMessage(Header header, List<Question> questions, List<Answer> answers)
{
    public Header Header { get; set; } = header;

    public List<Question> Questions { get; set; } = questions;

    public List<Answer> Answers { get; set; } = answers;


    // public ResourceRecord[] Answers { get; set; }
    // public ResourceRecord[] Authority { get; set; }
    // public ResourceRecord[] Additional { get; set; }
    public byte[] ToBytes()
    {
        var bytes = new List<byte>();

        bytes.AddRange(Header.ToBytes());

        // if (Header.QueryResponseIndicator == false)
            foreach (var question in Questions)
                bytes.AddRange(question.ToBytes());

        // if (Header.QueryResponseIndicator == true)
            foreach (var answer in Answers)
                bytes.AddRange(answer.ToBytes());

        return bytes.ToArray();
    }

    public static DnsMessage Parse(BitArray bits)
    {
        var header = Header.Parse(bits);
        var questions = Question.ParseMany(bits, header.QuestionCount);
        var answers = Answer.ParseMany(bits, header.AnswerRecordCount);

        return new DnsMessage(header, questions, answers);
    }
}