<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="RoleInstance" HeaderText="Role Instance" />
            <asp:BoundField DataField="CounterName" HeaderText="Counter Name" />
            <asp:BoundField DataField="CounterValue" HeaderText="Counter Value" />
            <asp:BoundField DataField="EventDateTime" HeaderText="Event DateTime" />
        </Columns>
    </asp:GridView>
</asp:Content>
