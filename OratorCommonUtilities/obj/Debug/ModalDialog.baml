   M S B A M L   `   `   `  ����  OratorCommonUtilities'    !OratorCommonUtilities.ModalDialog   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$
  root��5      $	��200��6   $	��300��6   .��5      �  � DataContext 6   ��  �� ElementName$
 root��.+�5      �� 
������� J� Opacity$	 100Q�6@   y�6   �� *
Visibility+._�5      �� 
�������$��1q�6(   
������� �� CornerRadius$ 10,10,10,10��5	      $��Center=�6!   $��Center=�5
      ��6#   �� $
 root�� �� Path$ Height���6T   �� $
 root��$ Width� J� BitmapEffect 5      F� 5       F� Color$ Black��6-    F� Opacity$	 0.5Q�6;   	 F� 	Direction$		 270Q�6I   
 F� ShadowDepth$	
 0.7Q�6Y   ._�5      � $��10q�6    � RowDefinitions 5      �� 5      �� 5      $
��Auto �6   .+�5      �� -   5         ��Name$ MessageTextBlock��5      $
��Wrap=�6D   $��10q�6X    �� TextAlignment$
 Left=�6d   ��6+   �� *	Message+6d   A� 5      $��1��6   $��5q�6     A� Columns$ 3��6+   $��Center=�67   $��Bottom=�5   
   .+�5      �� -   5         ��Name$ ExportButton��5   !   $��Export��6R   $��2q�6c   �� 5      -   5        $ 
CopyButton��5   !   $
��Copy��6N   $��2q�6]   �� 5      -   5        $ CancelButton��5   !   $��Cancel��6R   $��2q�6c   5      5      5      5      5      5      