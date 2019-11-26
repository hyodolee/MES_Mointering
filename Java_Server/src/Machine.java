import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement(name = "machine")
@XmlAccessorType (XmlAccessType.FIELD)
public class Machine {
	private String machineName;                            // 장비명
	private String factoryName;                     // Factory명
	private String StateName;
    
    
	public String getStateName() {
		return StateName;
	}
	public void setStateName(String stateName) {
		StateName = stateName;
	}
	
	public String getMachineName() {
		return this.machineName;
	}
	public void setMachineName(String machineName) {
		this.machineName = machineName;
	}
	
	
	public String getfactoryName() {
		return this.factoryName;
	}
	public void setfactoryName(String factoryName) {
		this.factoryName = factoryName;
	}

    
}
