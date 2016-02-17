package org.projektkurs.src;

import java.awt.Canvas;
import java.awt.Color;
import java.awt.image.BufferedImage;

import javax.swing.JFrame;

public class Screen extends JFrame {

	public static final float Y = 0.5f;
	private static final long serialVersionUID = 1L;

	private Canvas canvas;
	private BufferedImage image;

	public Screen() {
		super();

		canvas = new Canvas();
		this.getContentPane().add(canvas);
		image = new BufferedImage(640, 480, BufferedImage.TYPE_INT_RGB);

		this.setBounds(100, 100, 640, 480);
		this.setTitle("Cam Viewer");
		this.setResizable(false);
		this.setLocationRelativeTo(null);
		this.setDefaultCloseOperation(DISPOSE_ON_CLOSE);
	}

	@Deprecated
	public void drawPixelYUV(int x, int y, int u, int v) {
		int b = (int) (Screen.Y + u / 0.493);
		int r = (int) (Screen.Y + v / 0.877);
		int g = (int) (1 / 0.587 * Screen.Y - 0.299 / 0.587 * r - 0.114 / 0.587 * b);
		if (r < 0) {
			r *= -1;
		}
		if (b < 0) {
			b *= -1;
		}
		if (g < 0) {
			g *= -1;
		}
		if (r > 255) {
			r = 255;
		}
		if (g > 255) {
			g = 255;
		}
		if (b > 255) {
			b = 255;
		}
		// System.out.println("U: " + u + " V: " + v + " R: " + r + " G: " + g
		// + " B: " + b);
		Color color = new Color(r, g, b);
		System.out.println("X:" + x + " Y: " + y);
		image.setRGB(x, y, color.getRGB());
	}

	public void drawPixelRGB(int x, int y, int r, int g, int b) {
//		if (r < 0) {
//			r *= -1;
//		}
//		if (b < 0) {
//			b *= -1;
//		}
//		if (g < 0) {
//			g *= -1;
//		}

		 r += 128;
		 g += 128;
		 b += 128;
		 System.out.println("R:" + r + " G: " + g + " B: " + b);

//		 r = 255-r;
//		 g = 255-g;
//		 b = 255-b;

		// if (r > 255) {
		// r = 255;
		// }
		// if (g > 255) {
		// g = 255;
		// }
		// if (b > 255) {
		// b = 255;
		// }
		// if (r < 0) {
		// r = 0;
		// }
		// if (g < 0) {
		// g = 0;
		// }
		// if (b < 0) {
		// b = 0;
		// }
		// System.out.print(b + ",");
		Color color = new Color(r, g, b);
		// System.out.println("X:" + x + " Y: " + y);
		image.setRGB(y, x, color.getRGB());
	}

	public void render() {
		if (this.isVisible()) {
			this.canvas.getGraphics().drawImage(image, 0, 0, null);
		}
	}

}