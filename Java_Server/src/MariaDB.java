import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;

public class MariaDB {
	private final String JDBC_DRIVER = "com.mysql.jdbc.Driver"; //����̹�
	private final String DB_URL = "jdbc:mysql://127.0.0.1/factory?&useUnicode=true&characterEncoding=UTF-8"; //������ DB ����
		
	private final String USER_NAME = "h551654"; //DB�� ������ ����� �̸��� ����� ����
	private final String PASSWORD = "551654"; //������� ��й�ȣ�� ����� ����
	
//	public static void main(String[] args) {
//		MariaDB test  = new MariaDB();
//	}
	
	public Machines getData(){ 
		Machines machines = new Machines();
		
	    Connection conn = null; 
		Statement state = null; 
	    try{
				Class.forName(JDBC_DRIVER);
				conn = DriverManager.getConnection(DB_URL, USER_NAME, PASSWORD);
				state = conn.createStatement();
				String sql; //SQL���� ������ String
				//sql = "select a.*,b.*,a.state as machine_state from machine a join event b on a.machine = b.machine";
				sql = "select a.* from machine a";
				ResultSet rs = state.executeQuery(sql); //SQL���� �����Ͽ� ����
				SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
				
				    while(rs.next()){
				    	Machine machine = new Machine();
				    	
						String machine_Name = rs.getString("machine");
						String machine_State = rs.getString("state");
						String machinegroup = rs.getString("machinegroup");
						String factory_Name = rs.getString("factory");
//						String eventnum = rs.getString("eventnum");
//						String eventname = rs.getString("eventname");
//						Date eventtime = rs.getTimestamp("eventtime");
						
						System.out.println("machine: "+ machine_Name + "\nstate: " + machine_State + "\nmachinegroup: " + machinegroup); 
//						System.out.println("eventnum: "+ eventnum + "\neventname: " + eventname + "\neventtime: " + sdf.format(eventtime)+"\n-------------\n");
				    	
						machine.setMachineName(machine_Name);
				    	machine.setStateName(machine_State);
				    	machine.setfactoryName(factory_Name);
				    	machines.getitems().add(machine);
					}
					
					rs.close();
					state.close();
					conn.close();
		} catch(Exception e){
			e.printStackTrace();

		} finally { //���ܰ� �ֵ� ���� ������ ����
			try{
				if(state!=null)
					state.close();
			}catch(SQLException ex1){
				//
			}
					
			try{
				if(conn!=null)
					conn.close();
			}catch(SQLException ex1){
				//
			}
		}
	    
	    return machines;
	}
	
	public void setData(String machineName, String setState){ 
	    Connection conn = null; 
	    PreparedStatement pstmt = null;
	    
	    String SQL = "UPDATE machine SET state=? WHERE  machine=?";

	    try{
				Class.forName(JDBC_DRIVER);
				conn = DriverManager.getConnection(DB_URL, USER_NAME, PASSWORD);
				pstmt  = conn.prepareStatement(SQL);
				
				pstmt.setString(1, setState); 
				pstmt.setString(2, machineName);
				
				pstmt.executeUpdate();   
				    
				pstmt.close();
				conn.close();
		} catch(Exception e){
			e.printStackTrace();

		} finally { //���ܰ� �ֵ� ���� ������ ����
			try{
				if(conn!=null)
					conn.close();
			}catch(SQLException ex1){
				//
			}
		}
	}
}
