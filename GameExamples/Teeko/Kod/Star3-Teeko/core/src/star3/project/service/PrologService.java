package star3.project.service;

import java.util.HashMap;
import java.util.Hashtable;

import jpl.*;

public class PrologService 
{
	public PrologService() 
	{
		
	}
	
	public void start()
	{  
	    Query q1 = new Query("consult('teeko.pl')");
		System.out.println(q1.hasSolution() ? "Plik wczytano pomyœlnie." : "Wyst¹pi³ b³¹d podczas wczytywania pliku.");
	}
	
	public void setCoord(String player)
	{
		String t4 = "rozstawienie('" + player + "')";
		Query q4 = new Query(t4);
		q4.oneSolution();
	}
	
	public void setCoordHuman(int position)
	{
		String t4 = "rozstawienieCzlowiek(" + position + ", 'B')";
		Query q4 = new Query(t4);
		q4.oneSolution();
	}
	
	public void setCoordLast()
	{
		String t4 = "rozstawienieOstatni('B')";
		Query q4 = new Query(t4);
		q4.oneSolution();
	}
	
	public int setCoordAI()
	{	
		 int x=0, y=0;
		 Hashtable solution;
		 
		 Query q5 = new Query(new Compound("rozstawienieAI", new Term[] { new Variable("PosX"), new Variable("PosY"), new Atom("R")}));
		 	 
		 while (q5.hasMoreSolutions())
		 {
		             solution = q5.nextSolution();
		             x = java.lang.Integer.parseInt(solution.get("PosX").toString());
		             y = java.lang.Integer.parseInt(solution.get("PosY").toString());
		 }
		
		return x + (y-1)*5;
	}
	
	public void move(String player)
	{
		String t4 = "wykonajRuch('" + player + "')";
		Query q4 = new Query(t4);
		q4.oneSolution();
	}
	
	public void moveHuman(String move)
	{
		String t4 = "wykonajRuchCzlowiek('B', '" + move + "')";
		Query q4 = new Query(t4);
		q4.oneSolution();
	}
	
	public int[] moveAI(HashMap<java.lang.Integer, String> playboard)
	{
		int x=0, y=0, xOld=0, yOld=0;
		String pawnID;
		 Hashtable solution;
		  
		 Query q5 = new Query(new Compound("wykonajRuchAI", new Term[] { new Variable("PosX"), new Variable("PosY"), new Variable("PosXO"), new Variable("PosYO")}));
		 	 
		 while (q5.hasMoreSolutions())
		 {
		             solution = q5.nextSolution();
		             x = java.lang.Integer.parseInt(solution.get("PosX").toString());
		             y = java.lang.Integer.parseInt(solution.get("PosY").toString());
		             xOld = java.lang.Integer.parseInt(solution.get("PosXO").toString());
		             yOld = java.lang.Integer.parseInt(solution.get("PosYO").toString());
		 }
		 
		 pawnID = playboard.get(xOld + (yOld-1)*5);
		 playboard.put(x + (y-1)*5, pawnID);
		 playboard.put(xOld + (yOld-1)*5, "");
		 
		 int[] tab = new int[2];
		 tab[0] = xOld + (yOld-1)*5;
		 tab[1] = x + (y-1)*5;
		 
		 return tab;
	}
}