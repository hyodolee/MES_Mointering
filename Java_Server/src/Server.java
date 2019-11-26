import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;

public class Server {
	
	//서버 소켓과 클라이언트 연결 소켓
	private ServerSocket serverSocket = null;
	private Socket clientSocket = null;
	
	//연결된 클라이언트 스레드를 관리하는 ArrayList
	ArrayList<MachineThread> machineList = new ArrayList<MachineThread>();
	
	public void start() {
		try {
			// 서버 소켓 생성
			serverSocket = new ServerSocket(8888);
            System.out.println("server start");
            // 무한루프를 돌면서 클라이언트 연결을 기다림
            while(true) {
            	clientSocket = serverSocket.accept( );
                // 연결된 클라이언트에서 스레드 클래스 생성
            	MachineThread chat = new MachineThread( );
                // 클라이언트 리스트 추가
            	machineList.add(chat);
                // 스레드 시작
                chat.start( );
            }
		}catch (Exception e) {
			System.out.println("[MultiChatServer]start( ) Exception 발생!!");
		}
	}
	
	public static void main(String[ ] args) {
        Server server = new Server( );
        server.start( );
    }
	
	public class MachineThread extends Thread {
		// 수신 메시지와 파싱 메시지를 처리하는 변수 선언
		String msg;
		String[ ] rmsg;
		byte[] bytes = new byte[100];
		int str_Len;
		
		// 입·출력 스트림 생성
		//private BufferedReader inMsg = null;
		//private PrintWriter outMsg = null;
		private InputStream is = null;
		private OutputStream os = null;
		private boolean status = true;
		
		@Override
		public void run() {
			super.run();
			
			try {
				// 입·출력 스트림 생성
				//inMsg = new BufferedReader(new InputStreamReader(clientSocket.getInputStream( )));
				//outMsg = new PrintWriter(clientSocket.getOutputStream( ), true);
				is = clientSocket.getInputStream();
				os = clientSocket.getOutputStream();
				
				while(status) {
					//수신된 메시지를 msg 변수에 저장
					//msg = inMsg.readLine();
					//System.out.println("server:" + msg);
					str_Len = is.read(bytes);
					msg = new String(bytes, 0, str_Len, "UTF-8");
					System.out.println("msg: " + msg);
					
					if(msg.equals("GETMACHINE"))
					{
						msg = "hd" + " " + "idle" + " " + "hd_factory" + " " + "area1";
					}
					else if(msg.equals("EXIT")) 
					{
						status = false;
						break;
					}
					msgSendAll(msg);
				}
				this.interrupt();
				System.out.println("(i)" + this.getName() + "Stop!");
			}catch (Exception e) {
				machineList.remove(this);
				e.printStackTrace();
			}
		}// end of run() on ChatThread
		void msgSendAll(String msg) {
			System.out.println("msgSendAll" + msg);
			for(MachineThread mt : machineList){
				//mt.outMsg.println(msg);
				try {
					mt.os.write(msg.getBytes("UTF-8"));
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
	}// end of Chat Thread
}// end of Multi Chat Server
