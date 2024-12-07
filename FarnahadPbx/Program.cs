using SIPSorcery.SIP;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("SIP Server Starting...");

        var sipChannel = new SIPUDPChannel(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 5060));

        var sipTransport = new SIPTransport();
        sipTransport.AddSIPChannel(sipChannel);

        // SIPDiagnostics.EnableTraceLogs();
        sipTransport.SIPTransportRequestReceived += async (localEndPoint, remoteEndPoint, sipRequest) =>
        {
            Console.WriteLine($"Request Received: {sipRequest.Method}");

            if (sipRequest.Method == SIPMethodsEnum.REGISTER)
            {
                Console.WriteLine($"Register request from: {sipRequest.Header.From.FromURI}");
                var response = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                await sipTransport.SendResponseAsync(response);
            }
            else if (sipRequest.Method == SIPMethodsEnum.INVITE)
            {
                Console.WriteLine($"Invite request from: {sipRequest.Header.From.FromURI}");
                var response = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Trying, null);
                await sipTransport.SendResponseAsync(response);

                response = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ringing, null);
                await sipTransport.SendResponseAsync(response);

                response = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                await sipTransport.SendResponseAsync(response);
            }
            else
            {
                var response = SIPResponse.GetResponse(sipRequest, SIPResponseStatusCodesEnum.MethodNotAllowed, null);
                await sipTransport.SendResponseAsync(response);
            }
        };

        Console.WriteLine("SIP Server is running. Press Ctrl+C to exit.");
        Console.ReadLine();

        sipTransport.Shutdown();
    }
}
