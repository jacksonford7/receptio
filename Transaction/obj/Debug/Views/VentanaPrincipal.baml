   M S B A M L   `   `   `  ����  Transaction(    "Transaction.Views.VentanaPrincipal   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4[
extToolkitJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  5   	   -   5          � Title$  TRANSACTION��5   	   $	��700��6   $
��1200��6*    � WindowStartupLocation$ CenterScreen=�67    � WindowStyle$
 None=�6\    � WindowState$ 	Maximized=�5	   	    � 	Resources 5         ContrasteSeleccion �� 
TargetType �� Property �� Value �� Triggers	 P� Property
 P� Value��%�   &	        �� 5   
   " ��6+   .�5      ��  � ��$ Georgia)�6+   �� 5       � ��
 �����6+    5      P� 5      	 � ��

 ���� . �5      ��  � �� �#19100763   5      5      5      .��5      �   StandardBackground#��[ 6   .+�5   
   .  (Microsoft.Windows.Controls.BusyIndicator     IsBusy 6#   �� *EstaOcupado+   BusyContent 6B   �� *MensajeBusy+.��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� .+�5       � -   5        $��frmContenedor��5      $
��Auto��61   $
��Auto��6>   $
��Left=�6L   $��0,0,0,0q�6g   $	��Top=�6x     � NavigationUIVisibility$ Hidden=�6�   5      �� 5      ���#E65100$��5q�6/   ._�5      ]� .+�5      �� $��Bottom=�6%   .+�5       �� $��50��6(   $	��350��64   $��35(�6@   $��Georgia)�6o   ��6N   �� *TipoTransaccion+6o   �� 5!      $ 
Horizontal=�6)   y�6B   �� *EsVisibleSensores+.+�5"   "   �� {�.   d� 	IsChecked 6+   �� *Input1+ �� Mode$ TwoWay=�6U   �� 5#   "   {�.   6+   �� *Input2+$ TwoWay=�6U   �� 5$   "   {�.   6+   �� *Input3+$ TwoWay=�6U   �� 5%   "   {�.   6+   �� *Input4+$ TwoWay=�6U   5&      �� 5'      $��70��6(   $	��350��64   $��15(�6@   $
��Wrap=�6N   ��6b   �� *	ListaTags+y�6}   �� *EsVisibleSensores+�� 5(      $��25��6(   $	��350��64   $��15(�6@   $
��Wrap=�6N   ��6b   �� *	TagReal+y�6{   �� *EsVisibleSensores+�� 5)      $��Kiosco��6(   $��70(�66   ���#191007$��Georgia)�6Y    �� TextAlignment$ Center=�6n   �� 5*      -   5        $	��250��5*   (   $ Center=�6S   $	��220(�6j   $��Microsoft Sans Serif)�6y   
�������$	��300��6�   ���#EF6C00��65   �� *NumeroKiosco+6�   �� 5+      $��30(�6?   ���#191007$ Center=�6b   ��6(   �� *Fecha+6b   �� 5,      -   5        $��../Imagenes/Logo.png��5,   $   $	��350��6B   6N   5-      �� 5.      $	��Top=�6%   .+�5/      �� $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *InicioBackground+.+�50   "   �� $��50��6(   $��75��64   $
��0,10q�6c   ��6?   �� *RutaImagenInicio+6c   �� 51   "   #���   6,   $��Datos��6�   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *InicioBackground+6�   52      �� 53      $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *HuellaBackground+y�6�   �� *EsVisibleHuella+.+�54   "   �� $��79��6(   $��0,10,0,0q�6W   ��63   �� *RutaImagenHuella+6W   �� 55   "   #���   6,   $��Huella��6�   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *HuellaBackground+6�   56      �� 57      $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *RfidBackground+.+�58   "   �� $��50��6(   $��77��64   $
��0,10q�6a   ��6?   �� *RutaImagenRfid+6a   �� 59   "   #���   6,   $	��Tag��6   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *RfidBackground+6�   5:      �� 5;      $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *ProcesoBackground+.+�5<   "   �� $��50��6(   $��79��64   $
��0,10q�6d   ��6?   �� *RutaImagenProceso+6d   �� 5=   "   #���   6,   $��Procesos��6�   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *ProcesoBackground+6�   5>      �� 5?      $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *TicketBackground+.+�5@   "   �� $��40��6(   $��78��64   $
��0,15q�6c   ��6?   �� *RutaImagenTicket+6c   �� 5A   "   #���   6,   $��Ticket��6�   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *TicketBackground+6�   5B      �� 5C      $ 
Horizontal=�6)   $��70��6B   $	��350��6N   $
��Left=�6�   ��6Z   �� *BarreraBackground+.+�5D   "   �� $��79��6(   $��0,10,0,2q�6X    J� RenderTransformOrigin$ 0.519,0.612&�6j   ��63   �� *RutaImagenBarrera+6j   �� 5E   "   #���   6,   $��Barrera��6�   $��50(�6�   $��Center=�6�   $��20,0,0,0q�6�   ��6Y   �� *BarreraBackground+6�   5F      5G      5H      5I      5J      5K      5L      5M      