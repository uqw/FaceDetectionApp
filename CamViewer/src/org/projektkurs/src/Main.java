package org.projektkurs.src;

import java.nio.ByteBuffer;
import java.util.ArrayList;

import com.aldebaran.qi.Application;
import com.aldebaran.qi.Session;
import com.aldebaran.qi.helper.proxies.ALVideoDevice;

public class Main {

	ALVideoDevice vdp;
	Screen screen;
	private boolean running;

	public static void main(String[] args) throws Exception {
		String robotUrl = "tcp://192.168.178.29:9559";
		// Create a new application
		Application application = new Application(args, robotUrl);
		// Start your application
		application.start();
		System.out.println("Succesfully connected to the Robot!");
		// Create an ALTextToSpeech object and link it to your current session
		Main reactor = new Main();

		reactor.run(application.session());
		application.run();

	}

	@SuppressWarnings("unchecked")
	private void run(Session session) throws Exception {
		vdp = new ALVideoDevice(session);
		screen = new Screen();
		screen.setVisible(true);
		String nameOfCam = vdp.subscribeCamera("Cam2", 0, 2, 11, 15);
		System.out.println(nameOfCam);
		running = true;
		while (running) {
			ArrayList<Object> image = (ArrayList<Object>) vdp
					.getImageRemote(nameOfCam);
			ByteBuffer imageData = (ByteBuffer) image.get(6);
			imageData.rewind();
			for (int j = 0; j < imageData.limit(); j +=3) {
				screen.drawPixelRGB(j/3 / 640,
						j/3 % 640, imageData.get(j),
						imageData.get(j+1),imageData.get(j+2));
			}
			System.out.println("Image");
			screen.render();
			if(!screen.isVisible()) {
				running = false;
			}
			vdp.releaseImage(nameOfCam);
		}
		vdp.unsubscribe(nameOfCam);
		System.exit(0);
	}

}
