import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;
import java.net.SocketException;

public class UDP_Server {
	
	interface OnReceiveCb{
		void onReceive(String data, DatagramPacket dp);
	}
	
	Socket Server;
	int port;
	DatagramSocket ds;
	
	OnReceiveCb myReceiveCb;
    
	public void setOnMyReceiveCb(OnReceiveCb cb) {
		myReceiveCb = cb;
	}
	
	public UDP_Server(int port) {
		this.port = port;
//        try {
//            DatagramSocket ds = new DatagramSocket(port);
//            while (true) {
//                byte buffer[] = new byte[512];
//                DatagramPacket dp = new DatagramPacket(buffer,buffer.length);
//                System.out.println("ready");
//                ds.receive(dp);
//                String str = new String(dp.getData());
//                System.out.println("수신된 데이터 : " + str);
//
//                InetAddress ia = dp.getAddress();
//                port = dp.getPort();
//                System.out.println("client ip : " + ia + " , client port : " + port);
//                //dp = new DatagramPacket(dp.getData(),dp.getData().length, ia,port);
//                dp = new DatagramPacket(dp.getData(),dp.getLength(), ia,port);
//                ds.send(dp);
//            }
//        } catch (IOException ioe) {
//            ioe.printStackTrace();
//        }
	}
	 
	public String StartAsServer(){
		try {
			ds = new DatagramSocket(port);
			ds.setBroadcast(true);
		} catch (SocketException e) {
			e.printStackTrace();
		}
		 return "success";
	}
	
	public String receiveMsg(DatagramPacket dp){
		String str = new String(dp.getData());
        System.out.println("수신된 데이터 : " + str);
        
        //등록된 event 호출
        if(myReceiveCb != null) {
        	myReceiveCb.onReceive(str.trim(), dp); //수신된 마지막 문자열 전달
        }
		
		return "success";
	}
	
	public void sendResponse(String data, DatagramPacket dp) {
		int client_Port;
		
		try {
			InetAddress ia = dp.getAddress();
			client_Port = dp.getPort();
	        System.out.println("client ip : " + ia + " , client port : " + client_Port);

	        dp = new DatagramPacket(data.getBytes(),data.getBytes().length, ia, client_Port);
			ds.send(dp);

		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void sendAll(String cmdExcute) {
		try {
	        DatagramPacket dp = new DatagramPacket(cmdExcute.getBytes(),cmdExcute.getBytes().length, InetAddress.getByName("255.255.255.255"), 5002);
			ds.send(dp);

		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
