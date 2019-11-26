import java.util.ArrayList;
import java.util.List;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement(name = "machines")
@XmlAccessorType(XmlAccessType.FIELD)
public class Machines {
	@XmlElement(name = "machine")
	private List<Machine> machine = null;
	
	public Machines() {
		machine = new ArrayList<Machine>();
	}

    public List<Machine> getitems() {
        return machine;
    }

    public void setitems(List<Machine> machine) {
        this.machine = machine;
    }
}
