import smtplib  # SMTP 사용을 위한 모듈
import re  # Regular Expression을 활용하기 위한 모듈
from email.mime.multipart import MIMEMultipart  # 메일의 Data 영역의 메시지를 만드는 모듈
from email.mime.text import MIMEText  # 메일의 본문 내용을 만드는 모듈
from email.mime.image import MIMEImage  # 메일의 이미지 파일을 base64 형식으로 변환하기 위한 모듈
 
class EMail():
    def __init__(self, _from, password) -> None:
        self._from = _from
        self.password = password

    def send(self, _to, title, text):
        reg = "^[a-zA-Z0-9.+_-]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$"  # 유효성 검사를 위한 정규표현식
        if re.match(reg, _to):
            msg = MIMEMultipart()
            msg['From'] = self._from
            msg['Subject'] = title
            msg['To'] = _to
            content_part = MIMEText(text, "plain")
            msg.attach(content_part)

            # ---- login
            gmail_smtp = "smtp.gmail.com"  # gmail smtp 주소
            gmail_port = 465  # gmail smtp 포트번호. 고정(변경 불가)
            smtp = smtplib.SMTP_SSL(gmail_smtp, gmail_port)
            smtp.login(self._from, self.password)
            smtp.sendmail(self._from, _to, msg.as_string())
            smtp.quit()
            print("정상적으로 메일이 발송되었습니다.")
        else:
            print("받으실 메일 주소를 정확히 입력하십시오.")