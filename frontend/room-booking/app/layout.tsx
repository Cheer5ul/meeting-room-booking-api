import Layout, { Footer, Content, Header } from "antd/es/layout/layout";
import "./globals.css";
import { Menu } from "antd";
import Link from "antd/es/typography/Link";

const items = [
  { key: "home", label: <Link href={"/"}>Home</Link> },
  { key: "rooms", label: <Link href={"/rooms"}>Rooms</Link> }
]

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <Layout style={{ minHeight: "100vh"}}>
          <Header>
            <Menu 
              theme="dark" 
              mode="horizontal" 
              items={items} 
              style={{ flex: 1, minWidth: 0}}
              />
          </Header>
          <Content style={{ padding: "0 48px" }}>{children}</Content>
          <Footer style={{ textAlign: "center"}}>
            Room Booking 2026 Created by Yurii Cherful
          </Footer>
          {children}
        </Layout>
        </body>
    </html>
  );
}
