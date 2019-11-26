import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringWriter;
import java.net.DatagramPacket;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;

public class Main {
	static UDP_Server server;
	static MariaDB db = new MariaDB();
	private static Machines machines;
	
	public static void main(String[] args) {
		BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
		String sCommand = null;
		
		CmdExcute("get db");
		
		do {
            System.out.print('#');
            
			try {
				sCommand = in.readLine();
			} catch (IOException e) {
				e.printStackTrace();
			}

            switch (sCommand.trim()){
                case "EXIT":
                	System.out.println("exit");
                	break;
                default:
                    String sResult = CmdExcute(sCommand);
                	System.out.println("default : " + sResult);
                    break;
            }

        } while (true);
	}
	
	public static String CmdExcute(String sCommand){
        String[] splitCommand = sCommand.toUpperCase().split(" ");

        switch (splitCommand[0])
        {
            case "EXIT":
                return "FAIL";
               
            case "GET":
            	if (splitCommand[1].equals("DB")){
            		//DB��  ��ȸ�� �ͼ� machines�� �߰�
            		machines = new Machines();
            		for(Machine machine : db.getData().getitems()) {
            				machines.getitems().add(machine);
                    }

                    return "DB ��ȸ�Ϸ�";
                }
            	break;
            case "SET":
                if (splitCommand[1].equals("MACHINE")){
                    String setMachineName = splitCommand[2];
                    if (setMachineName.length() > 0){
                        Machine newMachine = new Machine();
                        newMachine.setMachineName(splitCommand[2]);
                        newMachine.setfactoryName("hd_factory");
                        newMachine.setStateName("RUN");
                        
                        machines.getitems().add(newMachine);

                        return "SUCCESS";
                    }
                }else if (splitCommand[1].equals("MACHINESTATE")) {
//                    for (Machine machine : machines.getitems()) {
//                        if (machine.getMachineName().equals(splitCommand[2])){
//                            machine.setStateName(splitCommand[3]);
//                            
//                            return "SUCCESS";
//                        }
//                    }
                	db.setData(splitCommand[2], splitCommand[3]);
                	return "UPDATE SUCCESS";
                }else{
                	System.out.println("\"SET\":�ٽ� �Է¹ٶ��ϴ�." + splitCommand[1]);
                }
                break;
                
            case "VIEW":
                if (splitCommand[1].equals("MACHINE")){
                	String xmlString = null;
                    String findMachineName = splitCommand[2];
                    
                    for(Machine machine : machines.getitems()) {
                        if (machine.getMachineName().equals(findMachineName)){
                        	try {
        	                    JAXBContext jaxbContext = JAXBContext.newInstance(Machines.class);
        	                    Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
        	           
        	                    jaxbMarshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);
        	           
        	                    StringWriter sw = new StringWriter();
        						jaxbMarshaller.marshal(machine, sw);
        						xmlString = sw.toString();

        						} catch (JAXBException e) {
        							e.printStackTrace();
        						}
     	                    
                            return xmlString;
                        }
                    }
                }else if (splitCommand[1].equals("MACHINELIST")){
                	//CmdExcute("get db");
                	
                	String xmlString = null;
                	
                	try {
                		//File file = new File("C:\\hd\\machine.xml");
	                    JAXBContext jaxbContext = JAXBContext.newInstance(Machines.class);
	                    Marshaller jaxbMarshaller = jaxbContext.createMarshaller();
	           
	                    jaxbMarshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);
	           
	                    //jaxbMarshaller.marshal(machines, file);
	                    //jaxbMarshaller.marshal(machines, System.out);
	                    StringWriter sw = new StringWriter();
						jaxbMarshaller.marshal(machines, sw);
						xmlString = sw.toString();

						} catch (JAXBException e) {
							e.printStackTrace();
						}
                	
					return xmlString;
                }else
                {
                	System.out.println("VIEW" + "�ٽ� �Է¹ٶ��ϴ�." +splitCommand[1]);
                }
                break;
                
            case "START": /* UDP ��� Start  */
            	if (splitCommand[1].equals("UDP")){
	           		server = new UDP_Server(Integer.parseInt(splitCommand[2]));
	           		 
	           		UDP_Server.OnReceiveCb callback = new UDP_Server.OnReceiveCb() {
	
	           			@Override
						public void onReceive(String data, DatagramPacket dp) {
	           				String result = CmdExcute(data);
	           				System.out.println(result);
	           				
	           				server.sendResponse(result, dp);
						}
	           		};
	           		server.myReceiveCb = callback;
	           		
	           		server.StartAsServer();
	           		
                    return "UDP ��� Start SUCCESS";
                }else if(splitCommand[1].equals("SEND")) {
                	//autoView("view machinelist");
                	autoSendMessage();
                	return "machinelist ������ �Ϸ�";
                }
                break;
            default:
            	System.out.println("\"START\":�ٽ� �Է¹ٶ��ϴ�.");
                break;
        }

        return null;
    }
	
	//8�ʸ��� ������ machinelist ����
	private static void autoView(String str) {
		Timer timer = new Timer();
		TimerTask task = new TimerTask() {
			@Override
			public void run() {
				if(machines != null) {
					server.sendAll(CmdExcute(str));
					System.out.println("����");
				}
			}
		};
		
		timer.schedule(task, 5000,15000);	
	}
	
	//5�ʸ��� machine�� state�� �ٲ�.
	private static void autoSendMessage() {
		Timer timer = new Timer();
		TimerTask task = new TimerTask() {
			@Override
			public void run() {
				int size = machines.getitems().size();
				
				Random rand = new Random();
				int random = rand.nextInt(size);
				
				Machine machine = machines.getitems().get(random);
				
				String sChangeState = "";
				
				switch (machine.getStateName())
		        {
		            case "WAIT":
		                sChangeState = "RUN";
		                break;
		            case "RUN":
		                sChangeState = "WAIT";
		                break;
		            case "DOWN":
		                return;
		            default:
		                sChangeState = "WAIT";
		                break;
		        }
				
				String sCmdMsg = "set machinestate" + " "+ machine.getMachineName() + " " + sChangeState;
				
				String rtn = CmdExcute(sCmdMsg);
				System.out.println("autoSendMessage: " + rtn);
				
				CmdExcute("get db");
				
				if(machines != null) {
					server.sendAll(CmdExcute("view machinelist"));
					System.out.println("����");
				}
			}
		};
		
		timer.schedule(task, 5000,8000);
	}

}
