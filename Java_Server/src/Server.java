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
	
	//���� ���ϰ� Ŭ���̾�Ʈ ���� ����
	private ServerSocket serverSocket = null;
	private Socket clientSocket = null;
	
	//����� Ŭ���̾�Ʈ �����带 �����ϴ� ArrayList
	ArrayList<MachineThread> machineList = new ArrayList<MachineThread>();
	
	public void start() {
		try {
			// ���� ���� ����
			serverSocket = new ServerSocket(8888);
            System.out.println("server start");
            // ���ѷ����� ���鼭 Ŭ���̾�Ʈ ������ ��ٸ�
            while(true) {
            	clientSocket = serverSocket.accept( );
                // ����� Ŭ���̾�Ʈ���� ������ Ŭ���� ����
            	MachineThread chat = new MachineThread( );
                // Ŭ���̾�Ʈ ����Ʈ �߰�
            	machineList.add(chat);
                // ������ ����
                chat.start( );
            }
		}catch (Exception e) {
			System.out.println("[MultiChatServer]start( ) Exception �߻�!!");
		}
	}
	
	public static void main(String[ ] args) {
        Server server = new Server( );
        server.start( );
    }
	
	public class MachineThread extends Thread {
		// ���� �޽����� �Ľ� �޽����� ó���ϴ� ���� ����
		String msg;
		String[ ] rmsg;
		byte[] bytes = new byte[100];
		int str_Len;
		
		// �ԡ���� ��Ʈ�� ����
		//private BufferedReader inMsg = null;
		//private PrintWriter outMsg = null;
		private InputStream is = null;
		private OutputStream os = null;
		private boolean status = true;
		
		@Override
		public void run() {
			super.run();
			
			try {
				// �ԡ���� ��Ʈ�� ����
				//inMsg = new BufferedReader(new InputStreamReader(clientSocket.getInputStream( )));
				//outMsg = new PrintWriter(clientSocket.getOutputStream( ), true);
				is = clientSocket.getInputStream();
				os = clientSocket.getOutputStream();
				
				while(status) {
					//���ŵ� �޽����� msg ������ ����
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
