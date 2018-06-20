<?php
/**
 * Template Name: Custom SignalR Page
 */

get_header(); ?>
<div id="content-wrap" class="clearfix">
			<div id="content">
					<div class="post-wrap">
							<div class="box">
								<div class="frame"><!-- content -->
									<div class="post-content">
										<div class="panel panel-primary">
											<div class="panel-heading" id="accordion">
												<span class="glyphicon glyphicon-comment"></span> Chat
												<div class="btn-group pull-right">
													<span id="onlinehost"></span>
												</div>
											</div>
											<div class="panel-collapse collapse in" id="collapseOne">
												<div class="panel-body" id="chatcontent">
													<ul id="messagesList" class="chat"></ul>
												</div>
												<div class="panel-footer">
													<div class="input-group">
														<input type="hidden" value="visitor" id="txtuser" />
														<input id="txtmessage" maxlength="250" type="text" class="form-control input-sm" placeholder="Type your message here..." />
														<span class="input-group-btn">
															<button class="btn btn-info btn-sm" id="btnsendmessage">
																Send
															</button>
														</span>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>

							</div>
				</div>

		
			</div>

			
		</div><!--content-->


<?php
get_footer();
?>
<link rel="stylesheet" href="../lib/bootstrap/dist/css/bootstrap.css" />
<link rel="stylesheet" href="../css/site.css" />
<script type="text/javascript" src="../lib/bootstrap/dist/js/bootstrap.js"></script>
<script type="text/javascript" src="../lib/signalr/signalr.js"></script>
<script type="text/javascript" src="../js/chat.js"></script>