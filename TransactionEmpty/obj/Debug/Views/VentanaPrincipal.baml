   M S B A M L   `   `   `  ����  TransactionEmpty-    'TransactionEmpty.Views.VentanaPrincipal   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4[
extToolkitJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  5   	   -   5          � Title$  TRANSACTION EMPTY��5   	   $	��448��6#   $	��768��60    � WindowStartupLocation$ CenterScreen=�6<    � WindowStyle$
 None=�6a    � WindowState$ 	Maximized=�5   	    � 	Resources 5         ContrasteSeleccion �� 
TargetType �� Property �� Value �� Triggers	 P� Property
 P� Value��%�   &	        �� 5	   
   " ��6+   .�5
      ��  � ��$ Georgia)�6+   �� 5       � ��
 �����6+    5      P� 5      	 � ��

 ���� . �5      ��  � �� �#19100763   5      5      5      .��5      � 
�������.+�5   
   .  (Microsoft.Windows.Controls.BusyIndicator     IsBusy 6#   �� *EstaOcupado+   BusyContent 6B   �� *MensajeBusy+.��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� .+�5       � -   5        $��frmContenedor��5      $
��Auto��61   $
��Auto��6>   $
��Left=�6L   $��0,0,0,0q�6g   $	��Top=�6x     � NavigationUIVisibility$ Hidden=�6�   5      �� 5      ���#E65100$��5q�6/   ._�5      ]� .+�5      �� $��Bottom=�6%   .+�5      �� $��Kiosco��6(   $��50(�66   ���#191007$��Georgia)�6Y    �� TextAlignment$ Center=�6n   �� 5      $	��150��6(   $ Center=�6S   $	��130(�6j   $��Microsoft Sans Serif)�6y   
�������$	��200��6�   ���#EF6C00��65   �� *NumeroKiosco+6�   �� 5      $��20(�6?   ���#191007$ Center=�6b   ��6(   �� *Fecha+6b   �� 5       $��../Imagenes/Logo.png��6$   $	��220��6B   5!      �� 5"      $	��Top=�6%   .+�5#      �� $ 
Horizontal=�6)   $��70��6B   $	��220��6N   $
��Left=�6�   ��6Z   �� *InicioBackground+.+�5$   "   �� $��50��6(   $��75��64   $
��0,10q�6c   ��6?   �� *RutaImagenInicio+6c   �� 5%   "   #���   6,   $��Datos��6�   $��40(�6�   $��Center=�6�   $��5,0,0,0q�6�   ��6Y   �� *InicioBackground+6�   5&      �� 5'      $ 
Horizontal=�6)   $��70��6B   $	��220��6N   $
��Left=�6�   ��6Z   �� *TicketBackground+.+�5(   "   �� $��40��6(   $��78��64   $
��0,15q�6c   ��6?   �� *RutaImagenTicket+6c   �� 5)   "   #���   6,   $��Ticket��6�   $��40(�6�   $��Center=�6�   $��5,0,0,0q�6�   ��6Y   �� *TicketBackground+6�   5*      5+      5,      5-      5.      5/      50      51      