package star3.project.service;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.HashMap;
import java.util.Map.Entry;

public class JavaService 
{
	public JavaService() 
	{
		
	}
	
	public void putPawnHuman(HashMap<Integer, String> playboard, PrologService ps, int number, int xy) throws IOException
	{
		String position = Integer.toString(xy);
		
		try 
		{			
			String[] positionArray = position.split("");
			int[] xyArray = new int[positionArray.length];
			
			for (int i = 0; i < positionArray.length; i++) 
			{
				xyArray[i] = Integer.parseInt(positionArray[i]);
			}
			String[] playerTypeArray = playboard.get(xyArray[0] + (xyArray[1])).split("");

				//X + (Y-1)*5
				ps.setCoordHuman(Integer.parseInt(position));
				playboard.put((xyArray[0] + ((xyArray[1]-1)*5)), "B" + number);
			
		} 
		catch (NumberFormatException e) 
		{
		      System.out.println("Wprowadzona warto�� nie jest liczb�. Spr�buj ponownie wprowadzi� poprawn� warto��.");
		      putPawnHuman(playboard, ps, number, xy);
		}		
	}
	
	public int putPawnAI(HashMap<Integer, String> playboard, PrologService ps, int number)
	{
		int xy = ps.setCoordAI();
		playboard.put(xy, "R" + number);
		return xy;
	}
	
	public void movePawnHuman(HashMap<Integer, String> playboard, PrologService ps, int xyold, int xynew) throws IOException
	{	
		String positionold = Integer.toString(xyold);
		
		String[] positionoldArray = positionold.split("");
		int[] xyoldArray = new int[positionoldArray.length];
			
		for (int i = 0; i < positionoldArray.length; i++) 
		{
			xyoldArray[i] = Integer.parseInt(positionoldArray[i]);
		}
		
		String positionnew = Integer.toString(xynew);
		
		String[] positionnewArray = positionnew.split("");
		int[] xynewArray = new int[positionnewArray.length];
			
		for (int i = 0; i < positionnewArray.length; i++) 
		{
			xynewArray[i] = Integer.parseInt(positionnewArray[i]);
		}
		
		int n = xynewArray[0] + ((xynewArray[1]-1)*5);
		int o = xyoldArray[0] + ((xyoldArray[1]-1)*5);
		
		String direction = "";
		
		if(xynew == xyold-10)
		{
			direction = "l";
		}
		else if(xynew == xyold+10)
		{
			direction = "p";
		}
		else if(xynew == xyold-1)
		{
			direction = "g";
		}
		else if(xynew == xyold+1)
		{
			direction = "d";
		}
		else if(xynew == xyold-11)
		{
			direction = "gl";
		}
		else if(xynew == xyold-9)
		{
			direction = "gp";
		}
		else if(xynew == xyold+11)
		{
			direction = "dp";
		}
		else if(xynew == xyold+9)
		{
			direction = "dl";
		}
		
		
		
		String field=playboard.get(o);
		
			
			playboard.put(n, field);
			ps.moveHuman(field.toLowerCase() + direction.toLowerCase());
			playboard.put(o, "");

	}
	
	public int[] movePawnAI(HashMap<Integer, String> playboard, PrologService ps)
	{
		int[] xy = new int[2];
		xy = ps.moveAI(playboard);
		return xy;
	}
	
}
