import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;

public class MariaDB {
	private final String JDBC_DRIVER = "com.mysql.jdbc.Driver"; //드라이버
	private final String DB_URL = "jdbc:mysql://127.0.0.1/factory?&useUnicode=true&characterEncoding=UTF-8"; //접속할 DB 서버
		
	private final String USER_NAME = "h551654"; //DB에 접속할 사용자 이름을 상수로 정의
	private final String PASSWORD = "551654"; //사용자의 비밀번호를 상수로 정의
	
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
				String sql; //SQL문을 저장할 String
				//sql = "select a.*,b.*,a.state as machine_state from machine a join event b on a.machine = b.machine";
				sql = "select a.* from machine a";
				ResultSet rs = state.executeQuery(sql); //SQL문을 전달하여 실행
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

		} finally { //예외가 있든 없든 무조건 실행
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

		} finally { //예외가 있든 없든 무조건 실행
			try{
				if(conn!=null)
					conn.close();
			}catch(SQLException ex1){
				//
			}
		}
	}
}
